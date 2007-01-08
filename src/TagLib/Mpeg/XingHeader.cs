/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : xingheader.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Collections;
using System;

namespace TagLib.Mpeg
{
   public class XingHeader
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint frames;
      private uint size;
      private bool valid;

      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public XingHeader (ByteVector data)
      {
         frames = 0;
         size = 0;
         valid = false;
         Parse (data);
      }

      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public bool IsValid {get {return valid;}}
      public uint TotalFrames {get {return frames;}}
      public uint TotalSize {get {return size;}}

      //////////////////////////////////////////////////////////////////////////
      // public static methods
      //////////////////////////////////////////////////////////////////////////
      public static int XingHeaderOffset (Version v, ChannelMode c)
      {
         if (v == Version.Version1)
         {
            if (c == ChannelMode.SingleChannel)
               return 0x15;
            else
               return 0x24;
         }
         else
         {
            if (c == ChannelMode.SingleChannel)
               return 0x0D;
            else
               return 0x15;
         }
      }
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Parse (ByteVector data)
      {
         // Check to see if a valid Xing header is available.

         if(!data.StartsWith ("Xing"))
            return;

         // If the XingHeader doesn't contain the number of frames and the total stream
         // info it's invalid.

         if((data [7] & 0x01) == 0)
         {
            Debugger.Debug ("MPEG::XingHeader::parse() -- Xing header doesn't contain the total number of frames.");
            return;
         }

         if((data[7] & 0x02) == 0)
         {
            Debugger.Debug ("MPEG::XingHeader::parse() -- Xing header doesn't contain the total stream size.");
            return;
         }

         frames = data.Mid (8, 4).ToUInt ();
         size = data.Mid (12, 4).ToUInt ();

         valid = true;
      }
   }
}
