﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

#if UNITTEST

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace Nuclex.Audio.Metadata.Exceptions {

  /// <summary>Unit Test for the ProtocolLevelAlreadySetException class</summary>
  [TestFixture]
  public class ProtocolLevelAlreadySetExceptionTest {

    /// <summary>
    ///   Verifies that the exception's default constructor is working
    /// </summary>
    [Test]
    public void TestDefaultConstructor() {
      ProtocolLevelAlreadySetException testException =
        new ProtocolLevelAlreadySetException();

      string testExceptionString = testException.ToString();
      Assert.IsNotNull(testExceptionString);
    }

    /// <summary>
    ///   Checks whether the exception correctly stores its inner exception
    /// </summary>
    [Test]
    public void TestInnerException() {
      Exception inner = new Exception("This is a test");
      ProtocolLevelAlreadySetException testException =
        new ProtocolLevelAlreadySetException("Hello World", inner);

      Assert.AreSame(inner, testException.InnerException);
    }

    /// <summary>
    ///   Test whether the exception can be serialized
    /// </summary>
    [Test]
    public void TestSerialization() {
      BinaryFormatter formatter = new BinaryFormatter();

      using(MemoryStream memory = new MemoryStream()) {
        ProtocolLevelAlreadySetException exception1 =
          new ProtocolLevelAlreadySetException("Hello World");

        formatter.Serialize(memory, exception1);
        memory.Position = 0;
        object exception2 = formatter.Deserialize(memory);

        Assert.IsInstanceOf<ProtocolLevelAlreadySetException>(exception2);
        Assert.AreEqual(
          exception1.Message, ((ProtocolLevelAlreadySetException)exception2).Message
        );
      }
    }

  }

} // namespace Nuclex.Audio.Metadata.Exceptions

#endif // UNITTEST
