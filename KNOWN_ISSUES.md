# Problems (as of alpha release 0.4)

* Platform failures
  * Unity supports building for many platforms.  Windows and Linux work reasonably well, but MacOS builds are unusable due to poor performance (less than 1fps).  WebGL builds fail due to unknown shader problems.  The reasons for platform-specific failures are unknown at present.

* Poor performance
  * The texture size and polygon count of the scene should allow high performance even on integrated graphics.  In practice, the framerate is low and input lag is noticeable on some such machines, especially in Windows.  This may be due to the disabling of mipmaps on the main surface texture.

# Feature requests, plans, thoughts

* Additional documentation
  * Especially: details of the texture mapping conventions
  * More comments in the source

* Adjustable size for paintbrush
  * Need to determine (and implement) a good control interface with brush size preview.  This is complicated by the difference between hyperbolic radius and apparent size.

* Show where painting would happen
  * Preview the size and position of a spot of paint that would be drawn if LMB/Shift were pressed

* Continuous line of paint
  * Current implementation draws dots as often as raycast calculations are performed; this results in a dotted line with fast mouse movement or when painting on H^2 near the ideal boundary.  A better implementation will instead draw a thickened polyline with these points as vertices.  The discontinuity of the UV map for the main texture is a complication here, and significant shader modifications will be needed to support this.

* Adjustable view of H^2
  * e.g. toggle centering on face or on vertex
  * Easy to implement by adding a premultiplier to the hyperbolic plane shader

* Better surface rotation control
  * Rotating the surface with the mouse does not feel intuitive

* Support for painting *all* intersections of the beam with the surface
  * Should be selectable (single intersection vs multiple)
  * Want visible feedback about current mode (make laser beam larger, different color?)

* Support for selecting the background texture of the surface/H^2.
  * At a minimum, offer options like: Solid white, colored tiles

* Partially transparent surface
  * Might be easy to implement using selection of background texture with constant alpha
  * Would make "paint all intersections" more useful, easier to look at.

* Better keyboard input handling
  * Up/down events of hold-for-action keys are sometimes missed, resulting in inversion of defaults (e.g. stuck in grab mode with no keys pressed)
  * The keyboard input system should probably be redesigned so that Update() is fast and dispatches requests for slower items such as mode changes

* Improvements that would require major changes
  * Other genus 2 surfaces
  * Higher genus surfaces