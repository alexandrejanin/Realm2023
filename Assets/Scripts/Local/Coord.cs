using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public struct Coord : IComparable<Coord> {
    public int x, y, z;

    public int SquaredMagnitude => x * x + y * y + z * z;

    public float Magnitude => Mathf.Sqrt(SquaredMagnitude);
    public int MaxDimension => Mathf.Max(x.Abs(), y.Abs(), z.Abs());

    public Coord Normalize => new Coord(x.Sign(), y.Sign(), z.Sign());
    public Coord Abs => new Coord(x.Abs(), y.Abs(), z.Abs());

    public Coord[] GetAdjacent(bool includeSelf, bool diagonal) {
        var coords = new List<Coord>();
        if (includeSelf) coords.Add(this);
        coords.AddRange(new[] {this + Right, this + Left, this + Forward, this + Back});
        if (diagonal) coords.AddRange(new[] {this + Right + Forward, this + Left + Forward, this + Right + Back, this + Left + Back});
        return coords.ToArray();
    }

    public Coord(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public int ToDirectionIndex {
        get {
            if (x == 1 && y == 0 && z == 0) return 0; //Right
            if (x == -1 && y == 0 && z == 0) return 1; //Left
            if (x == 0 && y == 1 && z == 0) return 2; //Up
            if (x == 0 && y == -1 && z == 0) return 3; //Down
            if (x == 0 && y == 0 && z == 1) return 4; //Forward
            if (x == 0 && y == 0 && z == -1) return 5; //Back

            return -1;
        }
    }

    public bool IsDirection => ToDirectionIndex != -1;

    public static readonly Coord Zero = new Coord(0, 0, 0);
    public static readonly Coord One = new Coord(1, 1, 1);
    public static readonly Coord Up = new Coord(0, 1, 0);
    public static readonly Coord Down = new Coord(0, -1, 0);
    public static readonly Coord Left = new Coord(-1, 0, 0);
    public static readonly Coord Right = new Coord(1, 0, 0);
    public static readonly Coord Forward = new Coord(0, 0, 1);
    public static readonly Coord Back = new Coord(0, 0, -1);

    private static readonly Random DefaultRandom = new Random();

    public static Coord RandomRange(Coord a, Coord b, Random random = null) {
        if (random == null) random = DefaultRandom;
        return new Coord(random.Next(a.x, b.x), random.Next(a.y, b.y), random.Next(a.z, b.z));
    }

    public Coord(Vector3 vector) : this(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z)) { }

    public int CompareTo(Coord other) => SquaredMagnitude - other.SquaredMagnitude;

    public override string ToString() => "(" + x + ", " + y + ", " + z + ")";

    public static implicit operator Vector3(Coord a) => new Vector3(a.x, a.y, a.z);

    public static bool operator ==(Coord a, Coord b) => a.Equals(b);

    public static bool operator !=(Coord a, Coord b) => !(a == b);

    public static Coord operator +(Coord a, Coord b) => new Coord(a.x + b.x, a.y + b.y, a.z + b.z);

    public static Coord operator -(Coord a, Coord b) => a + -b;

    public static Coord operator *(int i, Coord c) => new Coord(c.x * i, c.y * i, c.z * i);
    public static Coord operator *(Coord c, int i) => i * c;

    public static Coord operator /(Coord c, int i) => new Coord(c.x / i, c.y / i, c.z / i);

    public static Coord operator -(Coord c) => new Coord(-c.x, -c.y, -c.z);

    public static Coord operator *(QuaternionInt q, Coord c) => new Coord(q.ToQuaternion() * c);

    public override bool Equals(object o) {
        if (!(o is Coord)) return false;
        var other = (Coord) o;
        return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode() {
        unchecked {
            var hashCode = x;
            hashCode = (hashCode * 397) ^ y;
            hashCode = (hashCode * 397) ^ z;
            return hashCode;
        }
    }
}

[Serializable]
public struct QuaternionInt {
    [Range(0, 7)] public int x, y, z;

    public QuaternionInt(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Quaternion ToQuaternion() => Quaternion.Euler(x * 45, y * 45, z * 45);
    public override string ToString() => x + ", " + y + ", " + z;
}