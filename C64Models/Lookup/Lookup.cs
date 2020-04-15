﻿using C64Studio.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace C64Studio.Lookup
{
  public static class Machines
  {
    //public static Machine C64 = new Machine() { Type = MachineType.C64, InitialBreakpointAddress = 0xE178, InitialBreakpointAddressCartridge = 0x8000 };
    public static Machine C64 = new Machine() { Type = MachineType.C64, InitialBreakpointAddress = 0xA871, InitialBreakpointAddressCartridge = 0x8000 };
    public static Machine VC20 = new Machine() { Type = MachineType.VC20, InitialBreakpointAddress = 0xFD3C, InitialBreakpointAddressCartridge = 0xA000 };
    public static Machine C128 = new Machine() { Type = MachineType.C128, InitialBreakpointAddress = 0xA871, InitialBreakpointAddressCartridge = 0x8000 };
    public static Machine PLUS4 = new Machine() { Type = MachineType.PLUS4, InitialBreakpointAddress = 0x8BBC, InitialBreakpointAddressCartridge = 0x8000 };
  }
}
