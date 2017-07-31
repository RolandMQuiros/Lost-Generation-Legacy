using System;
using System.Collections.Generic;

namespace LostGen {
	public interface IBoard {
		#region ActionEvents
		event Action<Pawn> PawnAdded;
        event Action<Pawn> PawnRemoved;
        event Action<BoardBlock, BoardBlock> BlockChanged;
        event Action<Dictionary<BoardBlock, BoardBlock>> BlocksChanged;
		#endregion ActionEvents
		#region BlockFunctions
		void SetBlock(BoardBlock block);
		void SetBlocks(IEnumerable<BoardBlock> blocks);
		BoardBlock GetBlock(Point point);
		bool InBounds(Point point);
		bool IsOpaque(Point point);
		bool IsSolid(Point point);
		#endregion BlockFunctions
		#region PawnFunctions
		IEnumerator<Pawn> GetPawnIterator();
		bool PawnExists(Pawn pawn);
		Pawn FindPawnByName(string name);
		IEnumerable<Pawn> FindPawnsByName(string name);
		IEnumerable<Pawn> PawnsAt(Point point);
		bool AddPawn(Pawn pawn);
		bool RemovePawn(Pawn pawn);
		bool SetPawnPosition(Pawn pawn, Point newPosition);
		#endregion PawnFunctions
		#region StepFunctions		
		void BeginTurn();
		Queue<PawnAction> Step(Queue<IPawnMessage> messages);
		Queue<PawnAction> Turn(Queue<IPawnMessage> messages);
		#endregion StepFunctions
	}
}