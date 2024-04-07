using System.Collections.Generic;
using UnityEngine;

public static class EditorCubesAlgorithm
{
    public static List<WallSideInfo[]> GetWallsFromCubes(Vector3Int[] cubes)
    {
        Cube[] c = new Cube[cubes.Length];
        for (int i = 0; i < cubes.Length; i++)
        {
            c[i] = new Cube()
            {
                x = cubes[i].x,
                y = cubes[i].y,
                z = cubes[i].z
            };
        }
        int Find(int x, int y, int z)
        {
            for (int i = 0; i < c.Length; i++)
                if (c[i].x == x && c[i].y == y && c[i].z == z)
                    return i;
            return -1;
        }
        for (int i = 0; i < c.Length; i++)
        {
            Cube cc = c[i];
            cc.wx = Find(cc.x + 1, cc.y, cc.z);
            cc.wxm = Find(cc.x - 1, cc.y, cc.z);
            cc.wy = Find(cc.x, cc.y + 1, cc.z);
            cc.wym = Find(cc.x, cc.y - 1, cc.z);
            cc.wz = Find(cc.x, cc.y, cc.z + 1);
            cc.wzm = Find(cc.x, cc.y, cc.z - 1);
            c[i] = cc;
        }
        List<Plate>[] pl = new List<Plate>[6]
        {
            new List<Plate>(), new List<Plate>(), new List<Plate>(), new List<Plate>(), new List<Plate>(), new List<Plate>()
        };
        for (int i = 0; i < c.Length; i++)
        {
            Cube cc = c[i];
            if (cc.wx == -1) pl[0].Add(new Plate() { x = cc.x + 1, y = cc.y, z = cc.z, s = 0 });
            if (cc.wy == -1) pl[1].Add(new Plate() { x = cc.x, y = cc.y + 1, z = cc.z, s = 1 });
            if (cc.wz == -1) pl[2].Add(new Plate() { x = cc.x, y = cc.y, z = cc.z + 1, s = 2 });
            if (cc.wxm == -1) pl[3].Add(new Plate() { x = cc.x, y = cc.y, z = cc.z, s = 3 });
            if (cc.wym == -1) pl[4].Add(new Plate() { x = cc.x, y = cc.y, z = cc.z, s = 4 });
            if (cc.wzm == -1) pl[5].Add(new Plate() { x = cc.x, y = cc.y, z = cc.z, s = 5 });
            c[i] = cc;
        }
        int FindWall(int s, int x, int y, int z)
        {
            List<Plate> tab = pl[s];
            for (int i = 0; i < tab.Count; i++)
            {
                Plate p = tab[i];
                if (p.x == x && p.y == y && p.z == z)
                {
                    return i;
                }
            }
            return -1;
        }
        for (int xi = 0; xi < 6; xi++)
        {
            for (int xj = 0; xj < pl[xi].Count; xj++)
            {
                Plate wall = pl[xi][xj];
                wall.dir = new Dir[4];
                for (int dir = 0; dir < 4; dir++)
                {
                    Dir found = null;
                    for (int angle = 1; angle >= -1; angle--)
                    {
                        CalcRet data = Calc(wall.s, dir, angle);
                        int fwallid = FindWall(data.targetS, wall.x + data.x, wall.y + data.y, wall.z + data.z);
                        if (fwallid != -1)
                        {
                            found = new Dir()
                            {
                                s = data.targetS,
                                dir = data.targetDir,
                                id = fwallid,
                                angle = angle
                            };
                            break;
                        }
                    }
                    wall.dir[dir] = found;
                }
                pl[xi][xj] = wall;
            }
        }
        int[] startCounts = new int[6];
        startCounts[0] = 0;
        for (int i = 1; i < 6; i++)
        {
            startCounts[i] = startCounts[i - 1] + pl[i - 1].Count;
        }
        List<WallSideInfo[]> plates = new List<WallSideInfo[]>();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < pl[i].Count; j++)
            {
                WallSideInfo[] dir = new WallSideInfo[4];
                for (int k = 0; k < 4; k++)
                {
                    Dir wdir = pl[i][j].dir[k];
                    dir[k] = new WallSideInfo()
                    {
                        ID = wdir.id + startCounts[wdir.s],
                        Angle = wdir.angle,
                        Dir = wdir.dir,
                    };
                }
                plates.Add(dir);
            }
        }
        return plates;
    }
    static int SetSign(int s, char sign)
    {
        return (s >= 3 ? s - 3 : s) + (sign == '+' ? 0 : 3);
    }
    static int Next(int s)
    {
        bool plus = s <= 2;
        return (plus ? s + 1 : s - 2) % 3 + (plus ? 0 : 3);
    }
    static int OppositeSign(int s)
    {
        return s <= 2 ? s + 3 : s - 3;
    }
    static int Table(int s, int dir)
    {
        switch (dir)
        {
            case 0: return SetSign(Next(s), '-');
            case 1: return OppositeSign(Next(Next(s)));
            case 2: return SetSign(Next(s), '+');
            case 3: return Next(Next(s));
        }
        return s;
    }
    static int OppositeDir(int dir)
    {
        switch (dir)
        {
            case 0: return 2;
            case 1: return 3;
            case 2: return 0;
            case 3: return 1;
        }
        return 0;
    }
    static CalcRet Calc(int s, int dir, int angle)
    {
        bool plus = s <= 2;
        CalcRet ret = new CalcRet();
        switch (angle)
        {
            case 0: ret.targetS = s; break;
            case 1: ret.targetS = Table(s, dir); break;
            case -1: ret.targetS = OppositeSign(Table(s, dir)); break;
        }
        if (angle == 0)
        {
            ret.targetDir = OppositeDir(dir);
        }
        else
        {
            if (dir == 0 || dir == 2)
            {
                if ((dir == 0 && plus) || (dir == 2 && !plus))
                    ret.targetDir = 1;
                else
                    ret.targetDir = 3;
            }
            else
            {
                if ((angle == -1 && plus) || (angle == 1 && !plus))
                    ret.targetDir = 0;
                else
                    ret.targetDir = 2;
            }
        }
        int v1, v2, v3;
        v1 = ((angle == -1 && plus) || (angle == 1 && !plus)) ? -1 : 0;
        v2 = dir == 0 ? 1 : (dir == 2 && angle == 0) ? -1 : 0;
        v3 = ((plus && dir == 1) || (!plus && dir == 3)) ? 1 : ((plus && dir == 3 && angle == 0) || (!plus && dir == 1 && angle == 0)) ? -1 : 0;

        ret.x = s % 3 == 0 ? v1 : (s - 1) % 3 == 0 ? v3 : v2;
        ret.y = s % 3 == 0 ? v2 : (s - 1) % 3 == 0 ? v1 : v3;
        ret.z = s % 3 == 0 ? v3 : (s - 1) % 3 == 0 ? v2 : v1;
        return ret;
    }
    struct Cube
    {
        public int x, y, z, wx, wxm, wy, wym, wz, wzm;
    }
    struct Plate
    {
        public int x, y, z, s;
        public Dir[] dir;
        // s: 0:x+ 1:y+ 2:z+ 3:x- 4:y- 5:z-
    }
    class Dir
    {
        public int s, dir, id, angle;
    }
    struct CalcRet
    {
        public int targetDir, targetS, x, y, z;
    }
}