# Osaka70
**T.E.A.M.** research is based on experimentation. In order to both get a better understanding of the phenomena we are studying and to test the tools of our workflow, we felt even the need to confront existing projects of kinetic architecture. 
As a first case study we have chosen to deal with an iconic project of recent architectural history: the pavilion for the 1970 World Expo in Osaka by Maurizio Sacripanti. This architecture, unfortunately never realized, was presented at the competition of ideas held in 1968 to select the building that would represent Italy at the international exhibition. 

## Built with 
“Osaka ‘70” is built using **Unity** as Game Engine. The reason for this choice was mainly for the implementation of ready-to-use VR technologies, which helped us in the early stage of the prototyping research phase. As the development process went further on, we were able to develop our custom Components and built a custom framework on the top of the Oculus’ one, especially for what concerned the hand tracking feature, which was yet experimental at the start time of the research project. Many tools we developed for the “Platform One” research project helped us to have a better view on what we would have done and how we would have implemented the tools needed to take Osaka ‘70 back to life: some of those tools were taken and brought in Osaka, and in fact Osaka ‘70 has many pieces in common with Platform One. 

Another framework which helped us in the early stage, and that we took some elements from, is the open-source **Mixed Reality Toolkit** from Microsoft: the high customization of each Component allowed us to further change and adapt some scripts to our own necessities. A key role was determined by the lightweight yet customizable shaders of the MRTK: the feeling of something real yet visible only in the virtual world was achievable thanks to the custom shaders of the framework. They also helped us to keep pretty high performances, keeping in mind the limited resources available in the first generation of the Oculus Quest. 

## Getting started 
### Prerequisites 
“Osaka ‘70” requires at least Unity 20219.4.28f to build and run. Android build support (both Android SDK and OpenJDK) module needs to be installed with Unity in order to build for the Oculus Quest platform (which is an Android-based device). Any other framework is already installed in the project. 

### Installation 

1. Download or clone the “Osaka70” repository. 
2. Open UnityHub launcher. Select the Project tab on the right side and select Open>Add project from disk. 
3. Select the folder called “Osaka70” containing the entire Unity project and open it. 
4. First opening should take a while since Unity has to build the local Library of assets cache, compiled shaders and so on. 
5. After the recompiling the project opens directly with LoadingScene scene as primary scene in the editor. 

## Usage 
### Customize the environment 

Many aspects of the experience can be customized because many features have been developed as highly changeable: 
Some parameters can be changed directly in Unity, since some of them have been exposed as public in their related script and can be found under their script component 
Other parameters can be changed only in code, since they build up a more complex structure related to many different behaviors [each relevant script has its own documentation] 

### Build your package 

Any running build packaged from Unity has to be exported as an Android Package (.apk). The project should be already properly set. Otherwise, check into Build Settings that: 
1. Android is selected as target platform. If not, select Android. 
2. Under Android settings on the right side of the panel, the Texture Compression must be set to ASTC value. 
3. If any setting has been changed, confirm changes selecting Switch Platform. A process of assets conversion starts. It should take some time depending on the hardware configuration of your computer. 
4. If Android platform is properly set, you should be able to build your own package: open the Build Settings panel, select Build (or Build and Run if your Oculus is connected to your computer and set in the Oculus app), select your output saving location on your computer and wait for the built process to complete. After that, you can upload your .apk in your Oculus Quest using the Command Prompt (on Windows) or Terminal (on Mac) or apps such as SideQuest or the Oculus Developer Hub. 

IMPORTANT: in order to upload your packages, Developer Mode for your Oculus Quest must be enabled! 

## License
Copyright (c) 2022 Poplab srl 
 
Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions: 
 
The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software. 
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE. 
