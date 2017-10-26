public class DoorwayObject : WallObject {
	public Doorway doorway;
	public override Wall Wall => doorway;
}