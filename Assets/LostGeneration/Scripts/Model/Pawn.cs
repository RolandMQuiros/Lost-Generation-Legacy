using System;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public class Pawn {
        private Board m_board;
        private List<Point> m_footprint = new List<Point>();

        public Point Position;
        
        public Pawn(Board board, Point position, IEnumerable<Point> footprint) {
            if (board == null) {
                throw new ArgumentNullException("board");
            }

            if (footprint == null) {
                m_footprint = new List<Point>();
            } else {
                m_footprint = new List<Point>(footprint);
            }
            
            if (m_footprint.Count == 0) {
                m_footprint.Add(Point.Zero);
            }

            m_board = board;
            Position = position;
        }

        public bool Collides(Point point) {
            bool collided = false;

            for (int i = 0; i < m_footprint.Count && !collided; i++) {
                collided = m_footprint[i] == point;
            }

            return collided;
        }
    }

}