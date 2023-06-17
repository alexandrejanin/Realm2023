public class WallObject : EntityObject {
    public virtual Wall Wall { get; set; }

    public override Entity Entity => Wall;
}