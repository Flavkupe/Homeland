﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Nuclex.Game")]
[assembly: AssemblyProduct("Nuclex.Game")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Nuclex Development Labs")]
[assembly: AssemblyCopyright("Copyright © Nuclex Development Labs 2007-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("176e98aa-09e3-4904-9c59-b9f79ee7cc88")]

#if UNITTEST
// This is required to allow NMock to mock internal interfaces or interfaces
// nested in internal classes.
[assembly: InternalsVisibleTo(NMock.Constants.InternalsVisibleToDynamicProxy)]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
