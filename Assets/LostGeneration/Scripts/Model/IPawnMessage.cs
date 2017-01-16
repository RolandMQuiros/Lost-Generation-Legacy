﻿namespace LostGen {
    public interface IPawnMessage {
        Pawn Source { get; }
        Pawn Target { get; }
        string Text { get; }
        bool IsCritical { get; }
    }
}