namespace LostGen {
	public class PawnComponent {
		public Pawn Pawn { get; private set; }
		public bool IsEnabled = true;
		
		public void OnAdded(Pawn to) {
			Pawn = to;
		}

		/// <summary>
        /// Called when the owning Pawn is completely constructed.  Use this to cache
		/// component references within the same Pawn.
        /// </summary>
		public virtual void Start() { }
		/// <summary>
        /// Called when the Board is completely constructed. Use this to initialize values
		/// dependent on other Pawns.
        /// </summary>
		public virtual void OnBoardReady() { }

		public virtual void OnCollisionEnter(Pawn other) { }
		public virtual void OnCollisionStay(Pawn other) { }
		public virtual void OnCollisionExit(Pawn other) { }
		public virtual void OnPushAction(PawnAction action) { }
		public virtual void BeforeClearActions() { }
		
		public virtual void BeginTurn() { }
		public virtual void EndTurn() { }

		public virtual void PreStep() { }
		public virtual void PreAction(PawnAction action) { }
		public virtual void PostAction(PawnAction action) { }
		public virtual void PostStep() { }
	}

}