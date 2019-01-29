using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EightDir {
    [OrthoDirAttribute("North",     0,  1,  0,      0,  0,  1,      0, -1,      0)] North,
    [OrthoDirAttribute("Northeast", 1,  1,  0,      1,  0,  1,      1, -1,      1)] Northeast,
    [OrthoDirAttribute("East",      1,  0,  0,      1,  0,  0,      1,  0,      2)] East,
    [OrthoDirAttribute("Southeast", 1, -1,  0,      1,  0, -1,      1,  1,      3)] Southeast,
    [OrthoDirAttribute("South",     0, -1,  0,      0,  0, -1,      0,  1,      4)] South,
    [OrthoDirAttribute("Southwest",-1, -1,  0,     -1,  0, -1,     -1,  1,      5)] Southwest,
    [OrthoDirAttribute("West",     -1,  0,  0,     -1,  0,  0,     -1,  0,      6)] West,
    [OrthoDirAttribute("Northwest",-1,  1,  0,     -1,  0,  1,     -1, -1,      7)] Northwest,
}

public class EightDirAttribute : Attribute {

    // this set is in tile space
    // (xy is public for optimization)
    public IntVector2 XY;
    public int X { get { return XY.x; } }
    public int Y { get { return XY.y; } }

    // 2D screenspace
    public IntVector3 Px2D { get; private set; }
    public int Px2DX { get { return Px2D.x; } }
    public int Px2DY { get { return Px2D.y; } }
    public int Px2DZ { get { return Px2D.z; } }

    // 3D screenspace
    public IntVector3 Px3D { get; private set; }
    public int Px3DX { get { return Px3D.x; } }
    public int Px3DY { get { return Px3D.y; } }
    public int Px3DZ { get { return Px3D.z; } }

    public int Ordinal { get; private set; }
    public string DirectionName { get; private set; }

    internal EightDirAttribute(string directionName,
            int px2DX, int px2DY, int px2DZ,
            int px3DX, int px3DY, int px3DZ,
            int dx, int dy, int ordinal) {
        this.XY = new IntVector2(dx, dy);
        this.Px2D = new IntVector3(px2DX, px2DY, px2DZ);
        this.Px3D = new IntVector3(px3DX, px3DY, px3DZ);
        this.Ordinal = ordinal;
        this.DirectionName = directionName;
    }
}

public static class EightDirExtensions {

    public static EightDir Parse(string directionName) {
        foreach (EightDir dir in System.Enum.GetValues(typeof(EightDir))) {
            if (dir.DirectionName().ToLower() == directionName.ToLower()) {
                return dir;
            }
        }

        Debug.Assert(false, "Could not find EightDir matching " + directionName);
        return EightDir.North;
    }

    public static int X(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().X; }
    public static int Y(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Y; }
    public static IntVector2 XY(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().XY; }

    public static int Px2DX(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Px2DX; }
    public static int Px2DY(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Px2DY; }
    public static int Px2DZ(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Px2DY; }
    public static IntVector3 Px2D(this EightDir dir) { return new IntVector3(dir.Px2DX(), dir.Px2DY(), dir.Px2DZ()); }

    public static int Px3DX(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Px3DX; }
    public static int Px3DY(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Px3DY; }
    public static int Px3DZ(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Px3DZ; }
    public static IntVector3 Px3D(this EightDir dir) { return new IntVector3(dir.Px3DX(), dir.Px3DY(), dir.Px3DZ()); }

    public static int Ordinal(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().Ordinal; }
    public static string DirectionName(this EightDir dir) { return dir.GetAttribute<OrthoDirAttribute>().DirectionName; }
}
