using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * The generic "thing on the map" class
 */
[RequireComponent(typeof(Dispatch))]
[DisallowMultipleComponent]
public abstract class MapEvent : MonoBehaviour {
    
    public const string EventEnabled = "enabled";
    public const string EventCollide = "collide";
    public const string EventInteract = "interact";
    public const string EventMove = "move";

    public const string PropertyUnit = "unit";
    public const string PropertyTarget = "target";
    private const string PropertyCondition = "show";
    private const string PropertyInteract = "onInteract";
    private const string PropertyCollide = "onCollide";

    private static readonly string TypeChara = "Character";

    // Editor properties
    public float tilesPerSecond = 2.0f;
    public bool Passable = true;
    public string LuaCondition;
    [TextArea(3, 6)] public string LuaOnInteract;
    [TextArea(3, 6)] public string LuaOnCollide;

    public LuaMapEvent luaObject { get; private set; }
    public Vector3 targetPositionPx { get; set; }
    public bool tracking { get; private set; }

    public Vector3 PositionPx {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }

    private Map _parent;
    public Map Parent {
        get {
            // this is wiped in update but we'll cache it across frames anyway
            if (_parent != null) {
                return _parent;
            }
            GameObject parent = gameObject;
            while (parent.transform.parent != null) {
                parent = parent.transform.parent.gameObject;
                Map map = parent.GetComponent<Map>();
                if (map != null) {
                    _parent = map;
                    return map;
                }
            }
            return null;
        }
    }

    // TODO: new project
    public IntVector2 positionXY {
        get {
            
            return new IntVector2(0, 0);
        }
        set {

        }
    }

    public ObjectLayer Layer {
        get {
            GameObject parent = gameObject;
            do {
                parent = parent.transform.parent.gameObject;
                ObjectLayer layer = parent.GetComponent<ObjectLayer>();
                if (layer != null) {
                    return layer;
                }
            } while (parent.transform.parent != null);
            return null;
        }
    }

    private int _layerIndex = -1;
    public int LayerIndex {
        get {
            // this is a perf optimization -- events can't change layer now
            if (_layerIndex != -1) {
                return _layerIndex;
            }
            for (int thisLayerIndex = 0; thisLayerIndex < Parent.transform.childCount; thisLayerIndex += 1) {
                if (Parent.transform.GetChild(thisLayerIndex).gameObject.GetComponent<ObjectLayer>() == Layer) {
                    _layerIndex = thisLayerIndex;
                    return thisLayerIndex;
                }
            }
            Assert.IsTrue(false);
            return -1;
        }
    }

    private bool switchEnabled = true;
    public bool SwitchEnabled {
        get {
            return switchEnabled;
        }
        set {
            if (value != switchEnabled) {
                GetComponent<Dispatch>().Signal(EventEnabled, value);
            }
            switchEnabled = value;
        }
    }

    // public abstract

    // if we moved in this direction, where in screenspace would we end up?
    public abstract Vector3 CalculateOffsetPositionPx(OrthoDir dir);

    // public

    public void Start() {
        luaObject = Global.Instance().Lua.CreateEvent(this);
        luaObject.Set(PropertyCollide, LuaOnCollide);
        luaObject.Set(PropertyInteract, LuaOnInteract);
        luaObject.Set(PropertyCondition, LuaCondition);

        if (GetComponent<AvatarEvent>() != null) {
            Global.Instance().Lua.RegisterAvatar(GetComponent<AvatarEvent>());
        }

        GetComponent<Dispatch>().RegisterListener(EventCollide, (object payload) => {
            OnCollide((AvatarEvent)payload);
        });
        GetComponent<Dispatch>().RegisterListener(EventInteract, (object payload) => {
            OnInteract((AvatarEvent)payload);
        });

        CheckEnabled();
    }

    public void Update() {
        SetDepth();
        CheckEnabled();

        // TODO: only clear this when we change scene for the avatar
        _parent = null;
    }

    public void OnValidate() {
        SetDepth();
    }

    public void CheckEnabled() {
        SwitchEnabled = luaObject.EvaluateBool(PropertyCondition, true);
    }

    public OrthoDir DirectionTo(MapEvent other) {
        return OrthoDirExtensions.DirectionOf(other.PositionPx - PositionPx);
    }

    public bool IsPassableBy(CharaEvent chara) {
        return Passable || !SwitchEnabled;
    }

    public bool ContainsPosition(IntVector2 loc) {
        // TODO: new project
        //if (GetComponent<RectangleObject>() == null) {
        //    return loc == Position;
        //}
        //IntVector2 pos1 = Position;
        //IntVector2 pos2 = Position;
        //pos2.x += (int)((GetComponent<RectangleObject>().TmxSize.x / Map.TileSizePx) - 1);
        //pos2.y += (int)((GetComponent<RectangleObject>().TmxSize.y / Map.TileSizePx) - 1);
        //return loc.x >= pos1.x && loc.x <= pos2.x && loc.y >= pos1.y && loc.y <= pos2.y;
        return false;
    }

    public void SetLocation(IntVector2 location) {
        // TODO: new project
        //this.position = location;
        //OnValidate();
    }

    // set the one xyz coordinate not controlled by arrow keys
    protected abstract void SetDepth();

    // called when the avatar stumbles into us
    // before the step if impassable, after if passable
    private void OnCollide(AvatarEvent avatar) {
        luaObject.Run(PropertyCollide);
    }

    // called when the avatar stumbles into us
    // facing us if impassable, on top of us if passable
    private void OnInteract(AvatarEvent avatar) {
        if (GetComponent<CharaEvent>() != null) {
            GetComponent<CharaEvent>().facing = DirectionTo(avatar.GetComponent<MapEvent>());
        }
        luaObject.Run(PropertyInteract);
    }

    private LuaScript ParseScript(string lua) {
        if (lua == null || lua.Length == 0) {
            return null;
        } else {
            return Global.Instance().Lua.CreateScript(lua);
        }
    }

    private LuaCondition ParseCondition(string lua) {
        if (lua == null || lua.Length == 0) {
            return null;
        } else {
            return Global.Instance().Lua.CreateCondition(lua);
        }
    }

    public IEnumerator StepRoutine(OrthoDir dir) {
        // TODO: new project
        //if (tracking) {
        //    yield break;
        //}
        //tracking = true;

        //MapEvent mapEvent = GetComponent<MapEvent>();
        //mapEvent.Position += dir.XY();
        //targetPositionPx = mapEvent.CalculateOffsetPositionPx(dir);
        //GetComponent<Dispatch>().Signal(EventMove, dir);

        //while (true) {
        //    if (tilesPerSecond > 0) {
        //        mapEvent.PositionPx = Vector3.MoveTowards(mapEvent.PositionPx, 
        //            targetPositionPx, 
        //            tilesPerSecond * Time.deltaTime);
        //    } else {
        //        // indicates warp speed, cap'n
        //        mapEvent.PositionPx = targetPositionPx;
        //    }


        //    // TODO: ugly, I think we actually want to handle this via prefabs now
        //    if (Global.Instance().Maps.Camera.target == GetComponent<MapEvent>()) {
        //        Global.Instance().Maps.Camera.ManualUpdate();
        //    }

        //    if (mapEvent.PositionPx == targetPositionPx) {
        //        tracking = false;
        //        break;
        //    } else {
        //        yield return null;
        //    }
        //}
        yield return null;
    }

    public IEnumerator StepMultiRoutine(OrthoDir dir, int count) {
        for (int i = 0; i < count; i += 1) {
            yield return StartCoroutine(StepRoutine(dir));
        }
    }
}
