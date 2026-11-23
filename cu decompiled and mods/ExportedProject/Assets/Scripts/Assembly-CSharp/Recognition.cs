public class Recognition
{
	public int min;

	public bool recognizable => min <= PlayerCamera.main.body.skills.INT;

	public Recognition(int min)
	{
		this.min = min;
	}
}
