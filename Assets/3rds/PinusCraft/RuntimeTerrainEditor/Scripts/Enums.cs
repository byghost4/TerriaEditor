namespace RuntimeTerrainEditor
{
    public enum BrushMode
    {
        RAISE,
        LOWER,
        FLATTEN,
        SMOOTH,
        PAINT_TEXTURE,
        PAINT_OBJECT,
        REMOVE_OBJECT,
    }

    public enum BrushModeCn
    {
        抬升,
        压低,
        压扁,
        平滑,
        绘制贴图,
        绘制物体,
        移除物体
    }

    public enum TerrainSize
    {
        Size128     = 128,
        Size256     = 256,
        Size512     = 512,
        Size1024    = 1024,
    }
}