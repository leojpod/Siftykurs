using System;
using Sifteo;
using SiftyKurs;

namespace SiftyKurs
{
  internal class CubeSwitchHandler
  {
    private readonly RollingCube rollingCube;
    private readonly Cube.Side rollingCubeSide;
    private readonly Cube futurRollingCube;
    private readonly Cube.Side futurRollingCubeSide;
    private int _number;
    private static int SwitchHandlerNumber = 0;

    internal CubeSwitchHandler(RollingCube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide){
      rollingCube = c;
      rollingCubeSide = side;
      futurRollingCube = neighbor;
      futurRollingCubeSide = neighborSide;
      _number = SwitchHandlerNumber++;
      Log.Debug("CubeSwitchHandler#"+_number+" created between: "+c.C.UniqueId+"=>"+side+" & "+neighbor.UniqueId+"=>"+neighborSide);
    }

    public void Handle(Cube c, int x, int y, int z){
      int[] tilt = Helper.NormalizeTilt(x, y, z);
      int t_x = tilt[0]; int t_y = tilt[1]; int t_z = tilt[2];

      if(t_z == 0){//i.e. if the cube is vertical
        //then the tilt must be on the right side to be interesting!
        switch(rollingCubeSide){
        case Cube.Side.BOTTOM:
          if(t_y == 1){
            SwitchCubes();
          }
          break;
        case Cube.Side.TOP:
          if(t_y == -1){
            SwitchCubes();
          }
          break;
        case Cube.Side.LEFT:
          if(t_x == -1){
            SwitchCubes();
          }
          break;
        case Cube.Side.RIGHT:
          if(t_x == 1){
            SwitchCubes();
          }
          break;
        default:
          break;
        }
      }//else nothing to do for now
    }

    public void HandleBorderEvent(Cube.Side s){
      if(s.Equals(rollingCubeSide)){
        //this is the right side!
        //let's switch cubes!
        Log.Debug("Switch required by the CubeSwitcher#"+_number+" between \n\t--->"+rollingCube.C.UniqueId+"=>"+rollingCubeSide+" & "+futurRollingCube.UniqueId+"=>"+futurRollingCubeSide);
        SwitchCubesSmoothly();
      }
    }

    public void Remove(Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide){
      //we don't care about the parameters...
      Remove ();
    }
    public void Remove(){
      /* the point is:
       * - the cubes are no more neighbour
       * so we remove this object from the tilt listener list (and the neighbor remove one)
       * because we don't want the ball to switch cube anymore!
       */
      Log.Debug("Removing the CubeSwitcher#"+_number+"...");
      rollingCube.C.TiltEvent -= this.Handle;
      rollingCube.BallOnBorderEvent -= this.HandleBorderEvent;
      rollingCube.C.NeighborRemoveEvent -= this.Remove;
    }

    private void SwitchCubes(){
      //makes the rolling cube a simple GameCube
      GameCube gc = new GameCube(rollingCube.C);
      gc.SetupCube();
      //make the neighbor the new rolling cube
      RollingCube rc = new RollingCube(futurRollingCube);
      rc.SetupCube();
    }

    private void SwitchCubesSmoothly(){
      /* for that we need to:
       * - calculate the speed values according to the other cube side
       * - calculate the initial coordinate of the ball on the other cube
       * - create new rolling cube, and that's it
       */
      int futur_x, futur_y;
      double futur_speed_x, futur_speed_y;

      Object futurCubeData = futurRollingCube.userData;
      if(!futurCubeData.GetType().Equals(typeof(GameCube))){
        throw new InvalidCastException("There is something wrong, the futur cube doesn't have the good type of userData...");
      }
      GameCube futurOldGameCube = (GameCube) futurCubeData;
      //let's see at which speed we should drop it:
      Angle angle = Helper.AngleBetweenNeighbor(rollingCubeSide, futurRollingCubeSide);
      switch(angle){
      case Angle.NOON:
        futur_speed_x = rollingCube.SpeedX;
        futur_speed_y = rollingCube.SpeedY;
        //for each case we prepare futur_x and futur_y
        //we will change one of them later but that's easier to check that later
        futur_x = rollingCube.X;
        futur_y = rollingCube.Y;
        break;
      case Angle.THREE:
        futur_speed_x = rollingCube.SpeedY;
        futur_speed_y = - rollingCube.SpeedX;

        futur_x = rollingCube.Y;
        futur_y = rollingCube.MaxX - rollingCube.X;
        break;
      case Angle.SIX:
        futur_speed_x = - rollingCube.SpeedX;
        futur_speed_y = - rollingCube.SpeedY;

        futur_x = rollingCube.MaxX - rollingCube.X;
        futur_y = rollingCube.MaxY - rollingCube.Y;
        break;
      case Angle.NINE:
        futur_speed_x = - rollingCube.SpeedY;
        futur_speed_y = rollingCube.SpeedX;

        futur_x = rollingCube.MaxY - rollingCube.Y;
        futur_y = rollingCube.X;
        break;
      default:
        throw new FormatException("the given angle is not valid!");
      }

      //let's see where to "drop" the ball excaclty in the next cube...
      switch(futurRollingCubeSide){
      case Cube.Side.BOTTOM:
        futur_y = futurOldGameCube.MaxY;
        break;
      case Cube.Side.LEFT:
        futur_x = futurOldGameCube.MinX;
        break;
      case Cube.Side.RIGHT:
        futur_x = futurOldGameCube.MaxX;
        break;
      case Cube.Side.TOP:
        futur_y = futurOldGameCube.MinY;
        break;
      default:
        throw new FormatException("the side of the neighbor is not valid!");
      }


      //makes the rolling cube a simple GameCube
      GameCube gc = new GameCube(rollingCube.C);
      gc.SetupCube();
      //make the neighbor the new rolling cube
      rollingCube.MoveToNewCube(futurRollingCube, futur_x, futur_y, futur_speed_x, futur_speed_y);
      Log.Debug("cube switched!");
      throw new Exception("the ball switched cubes!");
    }
  }
}

