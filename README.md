# Face-Detect-Recognition
It is a WPF C# project through this we can detect faces and also identify the faces. 

In this project we are trying to detect and identify faces through web cam capturing.

First of all for identification we will create a emplty person group by using a some group name. Then after we should create a person under our person group with the name of the person. Now we can register person photos under this person. We will register three photos against this person. Same as we will create a two more person under the same person group. So our group is ready to use. Now we can use this group to identify the faces. 


we will start a web cam to capture photo, then save this captured image as .jpg file. then we will call the Face API to detect the faces in captured image for this we will passing our captured image then we will handle the return request in our code. Then we will display the num count of detcted faces in the image. Then after for identification we will again call the Face API to identify detected faces, for this we will be pass our captured image along with our created person group name. So API will identify against our group photos. Then send back the identification result with their id as well as name. 
