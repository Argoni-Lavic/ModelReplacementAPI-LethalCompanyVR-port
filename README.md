# ModelReplacementAPI-LethalCompanyVR-port

how to use
-
* download the latest release and extract the contents to the your plugins folder which can be found in your BepInEx folder in the LethalCompany program files
* C:\Program Files (x86)\Steam\steamapps\common\Lethal Company\BepInEx\plugins

* Only players who will be playing in VR will need this version of the Model Replacement API. 

Known issues
-
* Ragdoll behaves strangely at times (problem with the origonal API)
* Blood decals are not currently visible on the ragdoll replacement. (problem with the origonal API)
* Dying at the company may make the dead individual respawn without a model, but it may also return at a later point in time. (problem with the origonal API)
* 3rd person emotes may behave erratically due to the port.
* some models may have problems as the shaders must be switched for VR compatible shaders.

Unknown issues
-
* Many Hours of "fun" for me when they are found.

how it works
-
* If LethalCompanyVR (LCVR) is present, the shader whitelist will be bypassed, and all models will be rebuilt to enable instancing and to use shaders compatible with LCVR's rendering system.
