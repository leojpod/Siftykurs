using System;
using Sifteo;


namespace SiftyKurs
{
  public enum Angle{
    NOON, THREE, SIX, NINE
  };
  public class Tilt{
    public int _x;
    public int X{
      get{return _x;}
      set{
        if(IsValidTiltValue(value)){
          _x = value;
        }else{
          throw new ArgumentException("the given value is not a valid tilt value: "+value+" is not in [-1,1]");
        }
      }
    }

    public int _y;
    public int Y{
      get{return _y;}
      set{
        if(IsValidTiltValue(value)){
          _y = value;
        }else{
          throw new ArgumentException("the given value is not a valid tilt value: "+value+" is not in [-1,1]");
        }
      }
    }

    public int _z;
    public int Z{
      get{return _z;}
      set{
        if(IsValidTiltValue(value)){
          _z = value;
        }else{
          throw new ArgumentException("the given value is not a valid tilt value: "+value+" is not in [-1,1]");
        }
      }
    }

    public Tilt(int[] tilt){
      if(tilt.Length != 3){
        throw new ArgumentException("The tilt must contain 3 and only 3 elements and the given tilt contain "+tilt.Length+" elements","tilt");
      }else{
        X = tilt[0];
        Y = tilt[1];
        Z = tilt[2];
      }
    }

    public Tilt(int x, int y, int z)
      : this(new int[3] {x,y,z})
    {
    }
    public Tilt() : this(0,0,0){}

    private bool IsValidTiltValue(int aValue){
      return aValue <= 1 && aValue >= -1;
    }
  }
  public class Helper
  {
    public static Tilt NormalizeTilt(int[] tilt){
      if(tilt.Length != 3){
        throw new ArgumentException("The tilt must contain 3 and only 3 elements and the given tilt contain "+tilt.Length+" elements","tilt");
      }else{
        return NormalizeTilt(tilt[0], tilt[1], tilt[2]);
      }
    }

    public static Tilt NormalizeTilt(int x, int y, int z){
      Tilt tilt = new Tilt();
      switch(x){
      case 0:
        tilt.X = -1;
        break;
      case 1:
        tilt.X = 0;
        break;
      case 2:
        tilt.X = 1;
        break;
      default:
        tilt.X = 418; //using teapot HTTP code as unexpected tilt value...
        break;
      }

      switch(y){
      case 0:
        tilt.Y = 1;
        break;
      case 1:
        tilt.Y = 0;
        break;
      case 2:
        tilt.Y = -1;
        break;
      default:
        tilt.Y = 418; //using teapot HTTP code as unexpected tilt value...
        break;
      }

      switch(z){
      case 0:
        tilt.Z = -1;
        break;
      case 1:
        tilt.Z = 0;
        break;
      case 2:
        tilt.Z = 1;
        break;
      default:
        tilt.Z = 418; //using teapot HTTP code as unexpected tilt value...
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

    public static void PrintNumberOnACube(Cube c, int number, bool inOrange){
      int digits = ((int) Math.Log10(number)) + 1;
      int totalWidth = digits*16;
      int currentX = (Cube.SCREEN_MAX_X - totalWidth)/2;
      int currentdigit;
      int y = (Cube.SCREEN_MAX_Y - 16) /2;
      Log.Debug("printing "+number+" on cube "+c.UniqueId+"\n\t digits->"+digits+"\tplacement->("+currentX+","+y+")");
      int otherdigits = number;
      for(int i = 0; i < digits; i++){
        currentdigit = otherdigits / (int)(Math.Pow(10,(digits-i-1)));
        PrintDigitOnACube(c, currentdigit, currentX, y, inOrange);
        currentX +=16;
      }
      //that should do it
    }

    public static void PrintDigitOnACube(Cube c, int digit, int x, int y, bool inColor){
      if(digit < 0 || digit > 9){
        throw new ArgumentException("the given digit is not a real one... (not in [0,9])");
      }else{
        c.Image( (inColor)? "numbersOrange" : "numbersBlack",x, y, 0, digit*16, 16, 16, 1, 0);
      }
    }
  }

}

