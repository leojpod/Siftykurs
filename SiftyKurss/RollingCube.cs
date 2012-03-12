using System;
using Sifteo;
using SiftyKurss;

namespace SiftyKurss
{
  public class RollingCube: GameCube
  {
    private int _x, _y;
    private int _height, _width;
    private string _stone;

    public RollingCube (Cube c): base(c)
    {
      _c.userData = "RollingCube";
      _x = Cube.SCREEN_WIDTH /2;
      _y = Cube.SCREEN_HEIGHT /2;
      _stone = "BallStone";
      _height = Cube.SCREEN_HEIGHT / 4;
      _width = Cube.SCREEN_WIDTH / 4;
      SetupCube ();
    }
   
    private void SetupCube ()
    {
      base.SetupCube();
      DrawStone (true);
     #region adding the events handling
      _c.TiltEvent += ChangeCoordinate;
      _c.NeighborAddEvent += PrepareCubeRolling;
     #endregion
      //that's it for now
    }

    private void PrepareCubeRolling (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {

    }
   
    private void ChangeCoordinate (Cube c, int x, int y, int z)
    {
      //tilt is just an extremly simple thing: each axis can only have 3 values
      // - 0 tilted on one side,
      // - 1 neutral,
      // - 2 tilted on the other side...
      // - except for z where 0 means that the cube is facing done, 2 facing up and 1 resting on one side
      //so it gave us a few options:
      // - we know where the stone is rolling by checking the value of x and y,
      // - z gaves us the "how much tilted information"
      // i.e. we wants something like :

      int[] tilt = Helper.NormalizeTilt (x, y, z);
      int t_x, t_y, t_z;
      t_x = tilt [0];
      t_y = tilt [1];
      t_z = tilt [2];
      int middle = Cube.SCREEN_WIDTH / 2;

      _x = (int)(middle * (1 + (t_x / (1.0 + t_z * t_z))) - t_x * (1 - t_z * t_z) * (_width / 2 + _borderSize));
      _y = (int)(middle * (1 + (t_y / (1.0 + t_z * t_z))) - t_y * (1 - t_z * t_z) * (_width / 2 + _borderSize));
      Log.Debug ("Normalized Tilt : x->" + t_x + ", y->" + t_y + ", z->" + t_z);
      DrawStone (true);
    }
   
    private void DrawStone (bool repaint)
    {
      base.DrawCube(false);
      int pic_x = _x - _width / 2;
      int pic_y = _y - _height / 2;
      Log.Debug ("placement values: x->" + _x + ", y->" + _y + ", pic_x->" + pic_x + ", pic_y->" + pic_y);
      _c.Image (_stone, pic_x, pic_y, 0, 0, _width, _height, 1, 0);
      if (repaint) {
        _c.Paint ();
      }
    }
  }
}

