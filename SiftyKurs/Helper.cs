using System;
using Sifteo;


namespace SiftyKurs
{
  public enum Angle{
    NOON, THREE, SIX, NINE
  };
  public class Helper
  {
    public static int[] NormalizeTilt(int x, int y, int z){
      int[] tilt = new int[3];
      switch(x){
      case 0:
        tilt[0] = -1;
        break;
      case 1:
        tilt[0] = 0;
        break;
      case 2:
        tilt[0] = 1;
        break;
      default:
        tilt[0] = 418; //using teapot HTTP code as unexpected tilt value...
        break;
      }

      switch(y){
      case 0:
        tilt[1] = 1;
        break;
      case 1:
        tilt[1] = 0;
        break;
      case 2:
        tilt[1] = -1;
        break;
      default:
        tilt[1] = 418; //using teapot HTTP code as unexpected tilt value...
        break;
      }

      switch(z){
      case 0:
        tilt[2] = -1;
        break;
      case 1:
        tilt[2] = 0;
        break;
      case 2:
        tilt[2] = 1;
        break;
      default:
        tilt[2] = 418; //using teapot HTTP code as unexpected tilt value...
        break;
      }

      return tilt;
    }

    public static Angle AngleBetweenNeighbor(Cube.Side mainSide, Cube.Side neighborSide){
      if(mainSide.Equals(neighborSide)){
        //the cube are "facing" eachother
        return Angle.SIX;
      }
      Cube.Side temp = NextSide(mainSide, true);
      if(temp.Equals(neighborSide)){
        return Angle.THREE;
      }
      temp = NextSide(temp, true);
      if(temp.Equals(neighborSide)){
        return Angle.NOON;
      }
      temp = NextSide(temp, true);
      if(temp.Equals(neighborSide)){
        return Angle.NINE;
      }
      throw new FormatException("Something is wrong with the given sides: "+mainSide+","+neighborSide);
    }

    public static Cube.Side NextSide(Cube.Side s, bool clockwise){
      switch(s){
      case Cube.Side.TOP:
        return (clockwise)? Cube.Side.RIGHT : Cube.Side.LEFT;
      case Cube.Side.RIGHT:
        return (clockwise)? Cube.Side.BOTTOM : Cube.Side.TOP;
      case Cube.Side.BOTTOM:
        return (clockwise)? Cube.Side.LEFT : Cube.Side.RIGHT;
      case Cube.Side.LEFT:
        return (clockwise)? Cube.Side.TOP : Cube.Side.BOTTOM;
      default:
        throw new FormatException("the given side doesn't exist!");
      }
    }
  }
}

