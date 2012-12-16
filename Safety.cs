using System;
using System.Collections.Generic;
using System.Linq;
using CultLib;
using CultLib.Input;
using Games.Pathfinding;
using SFML.Window;
using System.Collections;
using Tanis.Collections;

namespace ld25
{
    class Safety : Entity
    {
        public static readonly int[,] Danger = new int[32,32];
        public List<Entity> Entities = new List<Entity>(); 
        public Safety() : base(null, new Vector2f(0,0), Ld25.UserInterface, true)
        {
            Calc();
            new Ticker(Parent, new TimeSpan(0, 0, 0, 0, 250), Calc);
        }
        public static List<Vector2f> CalcPath(Vector2f start, Vector2f end)
        {
            var astar = new AStar();

			var goalNode = new AStarNode2D(null,null,0, (int)end.X, (int)end.Y);
			var startNode = new AStarNode2D(null,goalNode, 0, (int)start.X, (int)start.Y) {GoalNode = goalNode};
            astar.FindPath(startNode, goalNode);

            return astar.Solution.ToArray().ToList().Select(i => new Vector2f(((AStarNode2D) i).X, ((AStarNode2D) i).Y)).ToList();
        }

        public void Calc()
        {
            var dangers = new Dictionary<Vector2f, int>();
            foreach (var e in Ld25.Game.Entities.Where(e => e.GetType() == typeof(Asteroid)))
            {
                try
                {
                    dangers.Add(new Vector2f((int)Math.Round(e.Pos.X / 20), (int)Math.Round(e.Pos.Y / 20)), (int)e.Tex.Size.X / 10);
                    var pos2 = new Vector2f(e.Pos.X, e.Pos.Y) + Angle.GetStep(((Asteroid)e).MoveRot) * 64;
                    dangers.Add(new Vector2f((int)Math.Round(pos2.X / 20), (int)Math.Round(pos2.Y / 20)), (int)e.Tex.Size.X / 20);
                }
                catch{}
            }
            for (var y = 0; y < 32; y += 1)
            {
                for (var x = 0; x < 32; x += 1)
                {
                    var i = 0;
                    for (var r = -1; r <= 2; r++)
                        for (var t = -1; t <= 2; t++)
                        {
                            var vec = new Vector2f((int)x + r, (int)y + t);
                            if (dangers.ContainsKey(vec))
                            {
                                i += dangers[vec];
                            }
                        }
                    Danger[x, y] = i;
                }
            }
        }

    }

    public class AStarNode2D : AStarNode
    {
        public int X
        {
            get
            {
                return _fx;
            }
        }
        private readonly int _fx;
        public int Y { get; private set; }

        public AStarNode2D(AStarNode aParent, AStarNode aGoalNode, double aCost, int ax, int ay)
            : base(aParent, aGoalNode, aCost)
        {
            _fx = ax;
            Y = ay;
        }
        private void AddSuccessor(ArrayList aSuccessors, int ax, int ay)
        {
            if (ax < 0)
            {
                ax = 31;
            }
            if (ay < 0)
            {
                ay = 31;
            }
            if (ax > 31)
            {
                ax = 0;
            }
            if (ay > 31)
            {
                ay = 0;
            }
            var currentCost = Safety.Danger[ax, ay];
            if (currentCost == -1)
            {
                return;
            }
            var newNode = new AStarNode2D(this, GoalNode, Cost + currentCost, ax, ay);
            if (newNode.IsSameState(Parent))
            {
                return;
            }
            aSuccessors.Add(newNode);
        }
        public override bool IsSameState(AStarNode aNode)
        {
            if (aNode == null)
            {
                return false;
            }
            return ((((AStarNode2D)aNode).X == _fx) &&
                (((AStarNode2D)aNode).Y == Y));
        }
        public override void Calculate()
        {
            if (GoalNode != null)
            {
                double xd = _fx - ((AStarNode2D)GoalNode).X;
                double yd = Y - ((AStarNode2D)GoalNode).Y;
                GoalEstimate = Math.Max(Math.Abs(xd), Math.Abs(yd));
            }
            else
            {
                GoalEstimate = 0;
            }
        }
        public override void GetSuccessors(ArrayList aSuccessors)
        {
            aSuccessors.Clear();
            AddSuccessor(aSuccessors, _fx - 1, Y);
            AddSuccessor(aSuccessors, _fx, Y - 1);
            AddSuccessor(aSuccessors, _fx + 1, Y);
            AddSuccessor(aSuccessors, _fx, Y + 1);
        }
    }
}
namespace Games.Pathfinding {

	/// <summary>
	/// Base class for pathfinding nodes, it holds no actual information about the map. 
	/// An inherited class must be constructed from this class and all virtual methods must be 
	/// implemented. Note, that calling base() in the overridden methods is not needed.
	/// </summary>
	public class AStarNode : IComparable
	{
		#region Properties

		private AStarNode FParent;
		/// <summary>
		/// The parent of the node.
		/// </summary>
		public AStarNode Parent
		{
			get
			{
				return FParent;
			}
			set
			{
				FParent = value;
			}
		}

		/// <summary>
		/// The accumulative cost of the path until now.
		/// </summary>
		public double Cost 
		{
			set
			{
				FCost = value;
			}
			get
			{
				return FCost;
			}
		}
		private double FCost;

		/// <summary>
		/// The estimated cost to the goal from here.
		/// </summary>
		public double GoalEstimate 
		{
			set
			{
				FGoalEstimate = value;
			}
			get 
			{
				Calculate();
				return(FGoalEstimate);
			}
		}
		private double FGoalEstimate;

		/// <summary>
		/// The cost plus the estimated cost to the goal from here.
		/// </summary>
		public double TotalCost
		{
			get 
			{
				return(Cost + GoalEstimate);
			}
		}

		/// <summary>
		/// The goal node.
		/// </summary>
		public AStarNode GoalNode 
		{
			set 
			{
				FGoalNode = value;
				Calculate();
			}
			get
			{
				return FGoalNode;
			}
		}
		private AStarNode FGoalNode;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="AParent">The node's parent</param>
		/// <param name="AGoalNode">The goal node</param>
		/// <param name="ACost">The accumulative cost until now</param>
		public AStarNode(AStarNode AParent,AStarNode AGoalNode,double ACost)
		{
			FParent = AParent;
			FCost = ACost;
			GoalNode = AGoalNode;
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Determines wheather the current node is the goal.
		/// </summary>
		/// <returns>Returns true if current node is the goal</returns>
		public bool IsGoal()
		{
			return IsSameState(FGoalNode);
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Determines wheather the current node is the same state as the on passed.
		/// </summary>
		/// <param name="ANode">AStarNode to compare the current node to</param>
		/// <returns>Returns true if they are the same state</returns>
		public virtual bool IsSameState(AStarNode ANode)
		{
			return false;
		}

		/// <summary>
		/// Calculates the estimated cost for the remaining trip to the goal.
		/// </summary>
		public virtual void Calculate()
		{
			FGoalEstimate = 0.0f;
		}

		/// <summary>
		/// Gets all successors nodes from the current node and adds them to the successor list
		/// </summary>
		/// <param name="ASuccessors">List in which the successors will be added</param>
		public virtual void GetSuccessors(ArrayList ASuccessors)
		{
		}

		/// <summary>
		/// Prints information about the current node
		/// </summary>
		public virtual void PrintNodeInfo()
		{
		}

		#endregion

		#region Overridden Methods

		public override bool Equals(object obj)
		{
			return IsSameState((AStarNode)obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			return(-TotalCost.CompareTo(((AStarNode)obj).TotalCost));
		}

		#endregion
	}
	
	/// <summary>
	/// Class for performing A* pathfinding
	/// </summary>
	public sealed class AStar
	{
		#region Private Fields

		private AStarNode FStartNode;
		private AStarNode FGoalNode;
		private Heap FOpenList;
		private Heap FClosedList;
		private ArrayList FSuccessors;

		#endregion

		#region Properties

		/// <summary>
		/// Holds the solution after pathfinding is done. <see>FindPath()</see>
		/// </summary>
		public ArrayList Solution
		{
			get 
			{
				return FSolution;
			}
		}
		private ArrayList FSolution;

		#endregion
		
		#region Constructors

		public AStar()
		{
			FOpenList = new Heap();
			FClosedList = new Heap();
			FSuccessors = new ArrayList();
			FSolution = new ArrayList();
		}

		#endregion

		#region Private Methods


		#endregion
		
		#region Public Methods

		/// <summary>
		/// Finds the shortest path from the start node to the goal node
		/// </summary>
		/// <param name="AStartNode">Start node</param>
		/// <param name="AGoalNode">Goal node</param>
		public void FindPath(AStarNode AStartNode,AStarNode AGoalNode)
		{
			FStartNode = AStartNode;
			FGoalNode = AGoalNode;

			FOpenList.Add(FStartNode);
			while(FOpenList.Count > 0) 
			{
				// Get the node with the lowest TotalCost
				AStarNode NodeCurrent = (AStarNode)FOpenList.Pop();

				// If the node is the goal copy the path to the solution array
				if(NodeCurrent.IsGoal()) {
					while(NodeCurrent != null) {
						FSolution.Insert(0,NodeCurrent);
						NodeCurrent = NodeCurrent.Parent;
					}
					break;					
				}

				// Get successors to the current node
				NodeCurrent.GetSuccessors(FSuccessors);
				foreach(AStarNode NodeSuccessor in FSuccessors) 
				{
					// Test if the currect successor node is on the open list, if it is and
					// the TotalCost is higher, we will throw away the current successor.
					AStarNode NodeOpen = null;
					if(FOpenList.Contains(NodeSuccessor))
						NodeOpen = (AStarNode)FOpenList[FOpenList.IndexOf(NodeSuccessor)];
					if((NodeOpen != null) && (NodeSuccessor.TotalCost > NodeOpen.TotalCost)) 
						continue;
					
					// Test if the currect successor node is on the closed list, if it is and
					// the TotalCost is higher, we will throw away the current successor.
					AStarNode NodeClosed = null;
					if(FClosedList.Contains(NodeSuccessor))
						NodeClosed = (AStarNode)FClosedList[FClosedList.IndexOf(NodeSuccessor)];
					if((NodeClosed != null) && (NodeSuccessor.TotalCost > NodeClosed.TotalCost)) 
						continue;
					
					// Remove the old successor from the open list
					FOpenList.Remove(NodeOpen);

					// Remove the old successor from the closed list
					FClosedList.Remove(NodeClosed);
					
					// Add the current successor to the open list
					FOpenList.Push(NodeSuccessor);
				}
				// Add the current node to the closed list
				FClosedList.Add(NodeCurrent);
			}
		}
		
		#endregion
	}
}
namespace Tanis.Collections
{
	/// <summary>
	/// The Heap allows to maintain a list sorted as long as needed.
	/// If no IComparer interface has been provided at construction, then the list expects the Objects to implement IComparer.
	/// If the list is not sorted it behaves like an ordinary list.
	/// When sorted, the list's "Add" method will put new objects at the right place.
	/// As well the "Contains" and "IndexOf" methods will perform a binary search.
	/// </summary>
	public class Heap : IList, ICloneable
	{
		#region Private Members

		private ArrayList FList;
		private IComparer FComparer = null;
		private bool FUseObjectsComparison;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// Since no IComparer is provided here, added objects must implement the IComparer interface.
		/// </summary>
		public Heap()
		{ 
			InitProperties(null, 0); 
		}

		/// <summary>
		/// Constructor.
		/// Since no IComparer is provided, added objects must implement the IComparer interface.
		/// </summary>
		/// <param name="Capacity">Capacity of the list (<see cref="ArrayList.Capacity">ArrayList.Capacity</see>)</param>
		public Heap(int Capacity)
		{ 
			InitProperties(null, Capacity); 
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="Comparer">Will be used to compare added elements for sort and search operations.</param>
		public Heap(IComparer Comparer)
		{ 
			InitProperties(Comparer, 0); 
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="Comparer">Will be used to compare added elements for sort and search operations.</param>
		/// <param name="Capacity">Capacity of the list (<see cref="ArrayList.Capacity">ArrayList.Capacity</see>)</param>
		public Heap(IComparer Comparer, int Capacity)
		{ 
			InitProperties(Comparer, Capacity); 
		}

		#endregion

		#region Properties

		/// <summary>
		/// If set to true, it will not be possible to add an object to the list if its value is already in the list.
		/// </summary>
		public bool AddDuplicates 
		{ 
			set 
			{ 
				FAddDuplicates = value; 
			} 
			get 
			{ 
				return FAddDuplicates; 
			} 
		}
		private bool FAddDuplicates;

		/// <summary>
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public int Capacity 
		{ 
			get 
			{
				return FList.Capacity; 
			} 
			set 
			{ 
				FList.Capacity = value; 
			} 
		}

		#endregion

		#region IList Members

		/// <summary>
		/// IList implementation.
		/// Gets object's value at a specified index.
		/// The set operation is impossible on a Heap.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Index is less than zero or Index is greater than Count.</exception>
		/// <exception cref="InvalidOperationException">[] operator cannot be used to set a value on a Heap.</exception>
		public object this[int Index]
		{
			get
			{
				if(Index >= FList.Count || Index < 0) 
					throw new ArgumentOutOfRangeException("Index is less than zero or Index is greater than Count.");
				return FList[Index];
			}
			set
			{
				throw new InvalidOperationException("[] operator cannot be used to set a value in a Heap.");
			}
		}

		/// <summary>
		/// IList implementation.
		/// Adds the object at the right place.
		/// </summary>
		/// <param name="O">The object to add.</param>
		/// <returns>The index where the object has been added.</returns>
		/// <exception cref="ArgumentException">The Heap is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
		public int Add(object O)
		{
			int Return = -1;
			if (ObjectIsCompliant(O))
			{
				int Index = IndexOf(O);
				int NewIndex = Index>=0 ? Index : -Index-1;
				if (NewIndex>=Count) FList.Add(O);
				else FList.Insert(NewIndex, O);
				Return = NewIndex;
			}
			return Return;
		}

		/// <summary>
		/// IList implementation.
		/// Search for a specified object in the list.
		/// If the list is sorted, a <see cref="ArrayList.BinarySearch">BinarySearch</see> is performed using IComparer interface.
		/// Else the <see cref="Equals">Object.Equals</see> implementation is used.
		/// </summary>
		/// <param name="O">The object to look for</param>
		/// <returns>true if the object is in the list, otherwise false.</returns>
		public bool Contains(object O)
		{
			return FList.BinarySearch(O, FComparer)>=0;
		}

		/// <summary>
		/// IList implementation.
		/// Returns the index of the specified object in the list.
		/// If the list is sorted, a <see cref="ArrayList.BinarySearch">BinarySearch</see> is performed using IComparer interface.
		/// Else the <see cref="Equals">Object.Equals</see> implementation of objects is used.
		/// </summary>
		/// <param name="O">The object to locate.</param>
		/// <returns>
		/// If the object has been found, a positive integer corresponding to its position.
		/// If the objects has not been found, a negative integer which is the bitwise complement of the index of the next element.
		/// </returns>
		public int IndexOf(object O)
		{
			int Result = -1;
			Result = FList.BinarySearch(O, FComparer);
			while(Result>0 && FList[Result-1].Equals(O))
				Result--;
			return Result;
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public bool IsFixedSize 
		{ 
			get 
			{ 
				return FList.IsFixedSize ; 
			} 
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public bool IsReadOnly 
		{ 
			get 
			{ 
				return FList.IsReadOnly; 
			} 
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public void Clear() 
		{ 
			FList.Clear(); 
		}

		/// <summary>
		/// IList implementation.
		/// Cannot be used on a Heap.
		/// </summary>
		/// <param name="Index">The index before which the object must be added.</param>
		/// <param name="O">The object to add.</param>
		/// <exception cref="InvalidOperationException">Insert method cannot be called on a Heap.</exception>
		public void Insert(int Index, object O)
		{
			throw new InvalidOperationException("Insert method cannot be called on a Heap.");
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <param name="Value">The object whose value must be removed if found in the list.</param>
		public void Remove(object Value) 
		{ 
			FList.Remove(Value); 
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <param name="Index">Index of object to remove.</param>
		public void RemoveAt(int Index) 
		{ 
			FList.RemoveAt(Index); 
		}

		#endregion

		#region IList.ICollection Members

		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(Array array, int arrayIndex) { FList.CopyTo(array, arrayIndex); }
		
		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public int Count { get { return FList.Count; } }

		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public bool IsSynchronized { get { return FList.IsSynchronized; } }

		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public object SyncRoot { get { return FList.SyncRoot; } }

		#endregion

		#region IList.IEnumerable Members

		/// <summary>
		/// IList.IEnumerable implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <returns>Enumerator on the list.</returns>
		public IEnumerator GetEnumerator()
		{ 
			return FList.GetEnumerator(); 
		}

		#endregion

		#region IClonable Members

		/// <summary>
		/// ICloneable implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <returns>Cloned object.</returns>
		public object Clone()
		{
			Heap Clone = new Heap(FComparer, FList.Capacity);
			Clone.FList = (ArrayList)FList.Clone();
			Clone.FAddDuplicates = FAddDuplicates;
			return Clone;
		}
		
		#endregion

		#region Delegate Members

		/// <summary>
		/// Defines an equality for two objects
		/// </summary>
		public delegate bool Equality(object Object1, object Object2);

		#endregion

		#region Overridden Members

		/// <summary>
		/// Object.ToString() override.
		/// Build a string to represent the list.
		/// </summary>
		/// <returns>The string refecting the list.</returns>
		public override string ToString()
		{
			string OutString = "{";
			for (int i=0; i<FList.Count; i++)
				OutString += FList[i].ToString() + (i != FList.Count-1 ? "; " : "}");
			return OutString;
		}

		/// <summary>
		/// Object.Equals() override.
		/// </summary>
		/// <returns>true if object is equal to this, otherwise false.</returns>
		public override bool Equals(object Object)
		{
			Heap SL = (Heap)Object;
			if ( SL.Count!=Count ) 
				return false;
			for (int i=0; i<Count; i++)
				if ( !SL[i].Equals(this[i]) ) 
					return false;
			return true;
		}

		/// <summary>
		/// Object.GetHashCode() override.
		/// </summary>
		/// <returns>Hash code for this.</returns>
		public override int GetHashCode() 
		{ 
			return FList.GetHashCode(); 
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Idem IndexOf(object), but starting at a specified position in the list
		/// </summary>
		/// <param name="Object">The object to locate.</param>
		/// <param name="Start">The index for start position.</param>
		/// <returns></returns>
		public int IndexOf(object Object, int Start)
		{
			int Result = -1;
			Result = FList.BinarySearch(Start, FList.Count-Start, Object, FComparer);
			while(Result > Start && FList[Result-1].Equals(Object))
				Result--;
			return Result;
		}

		/// <summary>
		/// Idem IndexOf(object), but with a specified equality function
		/// </summary>
		/// <param name="Object">The object to locate.</param>
		/// <param name="AreEqual">Equality function to use for the search.</param>
		/// <returns></returns>
		public int IndexOf(object Object, Equality AreEqual)
		{
			for (int i=0; i<FList.Count; i++)
				if ( AreEqual(FList[i], Object) ) return i;
			return -1;
		}

		/// <summary>
		/// Idem IndexOf(object), but with a start index and a specified equality function
		/// </summary>
		/// <param name="Object">The object to locate.</param>
		/// <param name="Start">The index for start position.</param>
		/// <param name="AreEqual">Equality function to use for the search.</param>
		/// <returns></returns>
		public int IndexOf(object Object, int Start, Equality AreEqual)
		{
			if ( Start<0 || Start>=FList.Count ) throw new ArgumentException("Start index must belong to [0; Count-1].");
			for (int i=Start; i<FList.Count; i++)
				if ( AreEqual(FList[i], Object) ) return i;
			return -1;
		}

		/// <summary>
		/// The objects will be added at the right place.
		/// </summary>
		/// <param name="C">The object to add.</param>
		/// <returns>The index where the object has been added.</returns>
		/// <exception cref="ArgumentException">The Heap is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
		public void AddRange(ICollection C)
		{
			foreach(object Object in C) 
				Add(Object);
		}

		/// <summary>
		/// Cannot be called on a Heap.
		/// </summary>
		/// <param name="Index">The index before which the objects must be added.</param>
		/// <param name="C">The object to add.</param>
		/// <exception cref="InvalidOperationException">Insert cannot be called on a Heap.</exception>
		public void InsertRange(int Index, ICollection C)
		{
			throw new InvalidOperationException("Insert cannot be called on a Heap.");
		}

		/// <summary>
		/// Limits the number of occurrences of a specified value.
		/// Same values are equals according to the Equals() method of objects in the list.
		/// The first occurrences encountered are kept.
		/// </summary>
		/// <param name="Value">Value whose occurrences number must be limited.</param>
		/// <param name="NumberToKeep">Number of occurrences to keep</param>
		public void LimitOccurrences(object Value, int NumberToKeep)
		{
			if(Value == null) 
				throw new ArgumentNullException("Value");
			int Pos = 0;
			while((Pos = IndexOf(Value, Pos)) >= 0)
			{
				if(NumberToKeep <= 0)
					FList.RemoveAt(Pos);
				else
				{
					Pos++; 
					NumberToKeep--; 
				}
				if(FComparer.Compare(FList[Pos],Value) > 0 ) 
					break; 
			}
		}

		/// <summary>
		/// Removes all duplicates in the list.
		/// Each value encountered will have only one representant.
		/// </summary>
		public void RemoveDuplicates()
		{
			int PosIt;
			PosIt = 0;
			while(PosIt < Count-1)
			{
				if(FComparer.Compare(this[PosIt],this[PosIt+1]) == 0 ) 
					RemoveAt(PosIt);
				else 
					PosIt++;
			}
		}

		/// <summary>
		/// Returns the object of the list whose value is minimum
		/// </summary>
		/// <returns>The minimum object in the list</returns>
		public int IndexOfMin()
		{
			int RetInt = -1;
			if (FList.Count > 0)
			{
				RetInt = 0;
				object RetObj = FList[0];
			}
			return RetInt;
		}

		/// <summary>
		/// Returns the object of the list whose value is maximum
		/// </summary>
		/// <returns>The maximum object in the list</returns>
		public int IndexOfMax()
		{
			int RetInt = -1;
			if(FList.Count > 0)
			{
				RetInt = FList.Count-1;
				object RetObj = FList[FList.Count-1];
			}
			return RetInt;
		}

		/// <summary>
		/// Returns the topmost object on the list and removes it from the list
		/// </summary>
		/// <returns>Returns the topmost object on the list</returns>
		public object Pop()
		{
			if(FList.Count == 0)
				throw new InvalidOperationException("The heap is empty.");
			object Object = FList[Count-1];
			FList.RemoveAt(Count-1);
			return(Object);
		}

		/// <summary>
		/// Pushes an object on list. It will be inserted at the right spot.
		/// </summary>
		/// <param name="Object">Object to add to the list</param>
		/// <returns></returns>
		public int Push(object Object)
		{
			return(Add(Object));
		}

		#endregion

		#region Private Members

		private bool ObjectIsCompliant(object Object)
		{
			if(FUseObjectsComparison && !(Object is IComparable)) 
				throw new ArgumentException("The Heap is set to use the IComparable interface of objects, and the object to add does not implement the IComparable interface.");
			if(!FAddDuplicates && Contains(Object)) 
				return false;
			return true;
		}

		private class Comparison : IComparer
		{
			public int Compare(object Object1, object Object2)
			{
				IComparable C = Object1 as IComparable;
				return C.CompareTo(Object2);
			}
		}

		private void InitProperties(IComparer Comparer, int Capacity)
		{
			if(Comparer != null)
			{
				FComparer = Comparer;
				FUseObjectsComparison = false;
			}
			else
			{
				FComparer = new Comparison();
				FUseObjectsComparison = true;
			}
			FList = Capacity > 0 ? new ArrayList(Capacity) : new ArrayList();
			FAddDuplicates = true;
		}

		#endregion
	}
}