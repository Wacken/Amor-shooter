/*
 * base class for all available decorations.
 * 
 * use this where all possible Decorations are of interest (e.g. preferences)
 * 
 * when only a single type of decoration is relevant, use the child classes as variable type instead.
 */
public abstract class Decorations
{
    protected int ID;

    private static int nextID = 0;
    protected static int NextID { get { return nextID++; } }
    
    #region Equals + HashCode overrrides
    public override bool Equals(object other)
    {
        Decorations d_other = other as Decorations;
        if (d_other == null)
            return false;
        else return d_other.ID == ID;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}

public class HeadDecoration : Decorations
{
    public static HeadDecoration Zylinder = new HeadDecoration();
    public static HeadDecoration Wig = new HeadDecoration();
    public static HeadDecoration Sombrero = new HeadDecoration();

    protected HeadDecoration()
    {
        this.ID = Decorations.NextID;
    }
}

public class FaceDecorations : Decorations
{
    public static FaceDecorations Monocle = new FaceDecorations();
    public static FaceDecorations FakeGlasses = new FaceDecorations();

    protected FaceDecorations()
    {
        this.ID = Decorations.NextID;
    }
}

public class TorsoDecorations : Decorations
{
    public static TorsoDecorations Necklace = new TorsoDecorations();
    public static TorsoDecorations Flower = new TorsoDecorations();

    protected TorsoDecorations()
    {
        this.ID = Decorations.NextID;
    }
}


