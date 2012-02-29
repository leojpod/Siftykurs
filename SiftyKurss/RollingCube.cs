using System;
using Sifteo;

namespace SiftyKurss
{
	public class RollingCube
	{
		private Cube _c;
		private int _x, _y;
		private string _stone;
		private int[] _tilt;
		
		public Cube C {
			get {return _c;}
		}
		public RollingCube (Cube c)
		{
			_c = c;
			_x = 0; _y = 0;
			_stone = "stone";
			_tilt = _c.Tilt;
			SetupCube();
		}
		
		private void SetupCube(){
			_c.ClearEvents();
			DrawStone();
			#region adding the events handling
			_c.TiltEvent += ChangeCoordinate;
			_c.NeighborAddEvent += PrepareCubeRolling;
			#endregion
			//that's it for now
		}

		private void PrepareCubeRolling (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
		{
			
		}
		
		private void ChangeCoordinate(Cube c, int x, int y, int z){
			int[] delta = new int[3];
			int[] newTilt = new int{x, y, z};
			for(int i = 0; i < 3 ; i++){
				delta[i] = newTilt[i] - _tilt[i];
			}
			//TODO check in the API what the value x, y and z means
			//but for now let's do a simple:
			_x += delta[0];
			_y += delta[1];
			//adjust the tilt values to match the new ones
			_x_tilt = x; _y_tilt = y; _z_tilt = z;
		}
		
		private void DrawStone(){
			_c.FillScreen(new Color(0, 82, 0));
			//TODO add more drawing code here: we need a border that react to the neigbors.
			#region dirty drawing of a cube representing the stone .. .
			_c.FillRect(new Color(255, 82, 0),_x, _y, 20, 20);
			#endregion
		}
	}
}

