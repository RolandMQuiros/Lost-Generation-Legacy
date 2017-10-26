/**
GOAP Notes
----------

We need a central planner, or maybe a Leader PawnComponent

The Leader maintains shared knowledge between its Pawns. That is, Dijsktra Maps,
known Pawn Positions, etc.

Personality PawnComponents just contain plain personality stats.
The Leader Component assigns Goals to its Pawns.

Every Pawn has a Brain, which contains the GOAP Planner. It takes the SkillSet, finds which Decisions it can make with those
Skills, and generates plans to achieve goals set by the Leader.

Problem: How to use Skills in decision-making, without appending AI-specific stuff to the Skills.
Also, why not append AI stuff to skills? Only read-only stuff, not behavioral.
Skills at least need to provide Pre and Postcondition StateOffsets.

Nah.

We have a limited number of moves per turn. Use this to our advantage somehow?

At the start, we have knowledge of
    * Visible Pawn positions in the current turn
    * Seen Skills
Using this we can create Dijkstra Maps of
    * Opponent Pawn's potential locations after each turn
    * Hazardous regions based on AoE of enemy's skills

The Knowledge Component holds Djikstra Maps, known Pawn positions, last seen pawn positions
It's a pure POD component, so it can be used with both AI and player-controlled controllers.

The Leader Component takes the individual Knowledge of all pawns it's assigned to and aggregates it into the Leader's own
Knowledge.

The Brain Component picks goals based on either its or its Leader's Knowledge.


GOAP Example--------------
    .....#.....
    .@...+...c.
    .....#.....
Goal: Move to the door, open the door, move to the coin, pick up the coin

First, how did we choose the goal?
* Qud has a Bored goal which hardcodes into a hostile state
* Brogue would put both the coin and other items of interest on the same Dijkstra map and let fate decide which one to
  approach. I suppose it checks a list of potential moves in its current position at the start of each turn to decide
  whether to pick up or attack.

* GOAP already assumes the goal was picked. It just chooses the actions to achieve that goal.

Like Qud, start with a Bored state.
From Knowledge, check what Goal states we could potentially choose from. 
    * If known enemies exist and are within vision, has offensive skill, KillTarget goal
    * If known enemies exist and are within vision, has defensive skill, Fortify goal
    * If known enemies exist and are outside vision, Patrol goal
    * If known treasure exists and are within vision, Approach
If more than one potential Goal is in the pool, choose one based on either the individual Pawn or Leader's priorities
and personality.

We want a Leader system that can
* Effectively assign goals based on Knowledge and Pawn characteristics
* Effectively assign goals based on mission objectives
* Diverge from the above based on personality or conflicting overarching goals
-------
Instead of a Leader or Brain, have a Planner
A Planner chooses its actions based on Knowledge, which can be shared. The Planner determines how this is done.
The Planner can have leader Planners, which provide their Knowledge.
If the leaders are killed, the Planner falls back on its own Knowledge.
-------
Everything in the game is based around skills, so isn't it natural to generate Dijkstra maps based on using those skills?
At the start of each turn, for each skill, generate a Dijkstra map that would represent the best place to use that skill
for some baseline utility.
e.g. for Melee attack, generate a map that places the attacker in an optimum place around the potential area the
     target could be

Using Qud's Goal Stack,
1. Pick a Root Goal based on mission objective*, leader's priorities, and own personality values
2. Root Goal prioritizes Subgoals based on some stats**
   Subgoals are just ways to use a Skill. You'll probably only fit one or two in each turn.
        * ApproachOnFoot - WalkSkill (Aggression 2)
        * Retreat - WalkSkill (Aggression -1)
        * AttackWithMelee - MeleeSkill (Aggression 3)
    - Remove the GOAP aspect of IDecisions? Or make them Goals? GOTO GOAL_EXAMPLE 
3. Skill generates a Dijkstra Map that tells the user the best way to use it. This can be shared knowledge.
    - Consider movement skills as 'subservient' actions
4. 

GOAL_EXAMPLE
------------
    .....#.....
    .@...+...c.
    .....#.....
Root Goal: Find Money Silently
    Action: Skill.PickUp(Adjacent Cell)
Sub Goal: ApproachOnFoot(Target = c, ) -> Moves 3 cells east

-------
Qud's Goal Stack is fundamentally the same thing as a back-pathing GOAP implementation, except the decisions and the
goal states are hard-coded in.  It's just a neat structure for a state machine!

Actually, Qud is just a hard-coded behavior tree! GOAP is a Planner! These are distinct concepts.
That idea you had about combining the two is basically a Hierarchial Task Network.


Would this actually be better suited for you? Instead of connecting states by StateOffset comparisons, just have them
coded in.
    + This makes things easier to debug
    + More handcrafted design
    + Goal fallbacks, so re-evaluating doesn't mean rebuilding the graph
    - Fewer emergent opportunities
    - Fewer randomization opportunities

-------
*/
namespace LostGen {
    public class Brain : PawnComponent {

    }
}