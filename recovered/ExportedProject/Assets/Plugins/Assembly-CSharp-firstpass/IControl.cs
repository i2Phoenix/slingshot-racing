public interface IControl : IAnimatable
{
	string Text { get; set; }

	bool IncludeTextInAutoCollider { get; set; }

	object Data { get; set; }

	void UpdateCollider();

	void Copy(IControl c);

	void Copy(IControl c, ControlCopyFlags flags);
}
