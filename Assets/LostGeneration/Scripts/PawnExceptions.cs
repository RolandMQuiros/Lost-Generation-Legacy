using System;

namespace LostGen {
    /// <summary>
    /// Throw this from PawnComponent.OnAdd when a required component is missing
    /// </summary>
    [Serializable]
    public class MissingComponentException<T> : Exception where T : PawnComponent {
        public MissingComponentException(Pawn pawn)
            : base("Pawn " + pawn + " is missing a required " + typeof(T).FullName) { }
    }

    [Serializable]
    public class NotStartedException : Exception {
        public NotStartedException(Pawn pawn)
            : base("Pawn " + pawn + " was not Start()ed before trying to access one of its components") { }
    }
}