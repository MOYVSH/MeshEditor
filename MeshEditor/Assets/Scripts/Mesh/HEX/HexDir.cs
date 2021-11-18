public class HexDir
{
    public static int round(int dir)
    {
        dir = dir % 6;
        if (dir < 0) dir += 6;
        return dir;
    }
}
