using System;

namespace SiftyKurss
{
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
  }
}

