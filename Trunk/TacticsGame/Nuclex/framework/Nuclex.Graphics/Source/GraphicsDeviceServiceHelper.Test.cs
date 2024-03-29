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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock;

using Nuclex.Testing.Xna;

namespace Nuclex.Graphics {

  /// <summary>Unit tests for the graphics device mock test helper</summary>
  [TestFixture]
  internal class GraphicsDeviceServiceHelperTest {

    #region interface IGraphicsDeviceServiceSubscriber

    /// <summary>Subscriber for the events of the graphics device service</summary>
    public interface IGraphicsDeviceServiceSubscriber {
      /// <summary>Called when a graphics device has been created</summary>
      /// <param name="sender">
      ///   Graphics device service that created a graphics device
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceCreated(object sender, EventArgs arguments);
      /// <summary>Called when a graphics device is about to be destroyed</summary>
      /// <param name="sender">
      ///   Graphics device service that is about to destroy its graphics device
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceDisposing(object sender, EventArgs arguments);
      /// <summary>Called when the graphics device is about to reset itself</summary>
      /// <param name="sender">
      ///   Graphics device service whose graphics device is about to reset itself
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceResetting(object sender, EventArgs arguments);
      /// <summary>Called when the graphics device has completed a reset</summary>
      /// <param name="sender">
      ///   Graphics device service whose graphics device has completed a reset
      /// </param>
      /// <param name="arguments">Not used</param>
      void DeviceReset(object sender, EventArgs arguments);
    }

    #endregion // interface IGraphicsDeviceSubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new MockFactory();
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.mockery != null) {
        this.mockery.Dispose();
        this.mockery = null;
      }
    }

    /// <summary>
    ///   Verifies that a created private service provider actually contains the service
    ///   it has been created for
    /// </summary>
    [Test]
    public void TestPrivateServiceProvider() {
      MockedGraphicsDeviceService originalService = new MockedGraphicsDeviceService();
      IServiceProvider provider = GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
        originalService
      );

      IGraphicsDeviceService service = (IGraphicsDeviceService)provider.GetService(
        typeof(IGraphicsDeviceService)
      );

      Assert.AreSame(originalService, service);
    }

    /// <summary>
    ///   Verifies that the dummy graphics device provide provides the graphics device
    ///   it has been given in its constructor
    /// </summary>
    [Test]
    public void TestDummyGraphicsDeviceService() {
      MockedGraphicsDeviceService originalService = new MockedGraphicsDeviceService();
      using(IDisposable keeper = originalService.CreateDevice()) {
        IGraphicsDeviceService dummyService;
        dummyService = GraphicsDeviceServiceHelper.MakeDummyGraphicsDeviceService(
          originalService.GraphicsDevice
        );
        try {
          Assert.AreSame(originalService.GraphicsDevice, dummyService.GraphicsDevice);
        }
        finally {
          IDisposable disposable = dummyService as IDisposable;
          if(disposable != null) {
            disposable.Dispose();
          }
        }
      }
    }

    /// <summary>
    ///   Tests whether the dummy graphics device service forwards the events being
    ///   issued by the wrapped graphics device
    /// </summary>
    [Test]
    public void TestDummyGraphicsDeviceServiceEvents() {
      MockedGraphicsDeviceService originalService = new MockedGraphicsDeviceService();
      originalService.CreateDevice();

      bool deviceExists = true;
      try {
        IGraphicsDeviceService dummyService;
        dummyService = GraphicsDeviceServiceHelper.MakeDummyGraphicsDeviceService(
          originalService.GraphicsDevice
        );
        Mock<IGraphicsDeviceServiceSubscriber> mockedSubscriber = mockSubscriber(dummyService);
        try {
          mockedSubscriber.Expects.One.Method(
            m => m.DeviceResetting(null, null)
          ).WithAnyArguments();
          mockedSubscriber.Expects.One.Method(
            m => m.DeviceReset(null, null)
          ).WithAnyArguments();

          originalService.ResetDevice();
          this.mockery.VerifyAllExpectationsHaveBeenMet();

          mockedSubscriber.Expects.One.Method(
            m => m.DeviceDisposing(null, null)
          ).WithAnyArguments();

          deviceExists = false;
          originalService.DestroyDevice();

          this.mockery.VerifyAllExpectationsHaveBeenMet();
        }
        finally {
          unmockSubscriber(dummyService, mockedSubscriber);
        }
      }
      finally {
        if(deviceExists) {
          originalService.DestroyDevice();
        }
      }
    }

    /// <summary>
    ///   Mocks a subscriber for the events of the mocked graphics device service
    /// </summary>
    /// <returns>The mocked event subscriber</returns>
    private Mock<IGraphicsDeviceServiceSubscriber> mockSubscriber(
      IGraphicsDeviceService graphicsDeviceService
    ) {
      Mock<IGraphicsDeviceServiceSubscriber> mockedSubscriber =
        this.mockery.CreateMock<IGraphicsDeviceServiceSubscriber>();

      graphicsDeviceService.DeviceCreated += new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceCreated
      );
      graphicsDeviceService.DeviceResetting += new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceResetting
      );
      graphicsDeviceService.DeviceReset += new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceReset
      );
      graphicsDeviceService.DeviceDisposing += new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceDisposing
      );

      return mockedSubscriber;
    }

    /// <summary>Finalizes a mocked graphics device service subscriber</summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service the mock in unsubscribed from
    /// </param>
    /// <param name="mockedSubscriber">Subscriber that will be unsubscribed</param>
    private void unmockSubscriber(
      IGraphicsDeviceService graphicsDeviceService,
      Mock<IGraphicsDeviceServiceSubscriber> mockedSubscriber
    ) {
      graphicsDeviceService.DeviceDisposing -= new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceDisposing
      );
      graphicsDeviceService.DeviceReset -= new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceReset
      );
      graphicsDeviceService.DeviceResetting -= new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceResetting
      );
      graphicsDeviceService.DeviceCreated -= new EventHandler<EventArgs>(
        mockedSubscriber.MockObject.DeviceCreated
      );
    }

    /// <summary>Mock object factory</summary>
    private MockFactory mockery;

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
