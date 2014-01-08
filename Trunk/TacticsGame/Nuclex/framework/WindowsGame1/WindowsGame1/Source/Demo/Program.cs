using System;

using Microsoft.Xna.Framework;

namespace WindowsGame1 {

  /// <summary>Containsthe program's startup code</summary>
  static class Program {
  
    /// <summary>Main entry point for the application</summary>
    static void Main(string[] args) {
      using(UserInterfaceDemoGame game = new UserInterfaceDemoGame()) {
        game.Run();
      }
    }

  }

} // namespace Nuclex.UserInterface.Demo
