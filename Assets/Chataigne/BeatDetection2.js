
/* ********** GENERAL SCRIPTING **********************

		This templates shows what you can do in this is module script
		All the code outside functions will be executed each time this script is loaded, meaning at file load, when hitting the "reload" button or when saving this file
*/


// You can add custom parameters to use in your script here, they will be replaced each time this script is saved
var beatThresholdSensibility = script.addFloatParameter("Beat Threshold Sensibility","Threshold value to decide if this is a beat or not. The lower it is, the more sensitive the algo will be.",0.5,5,30); 		//This will add a float number parameter (slider), default value of 0.1, with a range between 0 and 1
var sample = script.addIntParameter("Samples","Number of samples used for beat detection",1024,0,8192);
var analyzerValue = script.addTargetParameter("Analyzer value","Analyzer value from sound card"); 
var beatCountTrigger = script.addIntParameter("Beat Count Trigger","How many beat we need to launch anim",4,1,10);
var beatCountTriggerDuration = script.addFloatParameter("Beat Count Trigger Duration","Duration in seconds to reset beat count after last beat detected",4,1,10);
var beatCountIdleTime = script.addFloatParameter("Idle time","Idle time between 2 beat detection",0.3,0,1);


//Here are all the type of parameters you can create
/*
var myTrigger = script.addTrigger("My Trigger", "Trigger description"); 									//This will add a trigger (button)
var myBoolParam = script.addBoolParameter("My Bool Param","Description of my bool param",false); 			//This will add a boolean parameter (toggle), defaut unchecked
var myFloatParam = script.addFloatParameter("My Float Param","Description of my float param",.1,0,1); 		//This will add a float number parameter (slider), default value of 0.1, with a range between 0 and 1
var myIntParam = script.addIntParameter("My Int Param","Description of my int param",2,0,10); 				//This will add an integer number parameter (stepper), default value of 2, with a range between 0 and 10
var myStringParam = script.addStringParameter("My String Param","Description of my string param", "cool");	//This will add a string parameter (text field), default value is "cool"
var myColorParam = script.addColorParameter("My Color Param","Description of my color param",0xff0000ff); 	//This will add a color parameter (color picker), default value of opaque blue (ARGB)
var myP2DParam = script.addPoint2DParameter("My P2D Param","Description of my p2d param"); 					//This will add a point 2d parameter
var myP3DParam = script.addPoint3DParameter("My P3D Param","Description of my p3d param"); 					//This will add a point 3d parameter
var myTargetParam = script.addTargetParameter("My Target Param","Description of my target param"); 			//This will add a target parameter (to reference another parameter)
var myEnumParam = script.addEnumParameter("My Enum Param","Description of my enum param",					//This will add a enum parameter (dropdown with options)
											"Option 1", 1,													//Each pair of values after the first 2 arguments define an option and its linked data
											"Option 2", 5,												    //First argument of an option is the label (string)
											"Option 3", "banana"											//Second argument is the value, it can be whatever you want
											); 	
*/


//you can also declare custom internal variable
//var myValue = 5;
var actualEnergy = 0;
var actualAverageEnergy = 0;
var energyHistory = [];
var energyHistoryIndex = 0;
var beatTimeStamp = 0;
var beatCount = 0;

/*
 The init() function will allow you to init everything you want after the script has been checked and loaded
 WARNING it also means that if you change values of your parameters by hand and set their values inside the init() function, they will be reset to this value each time the script is reloaded !
*/
function init()
{
	//myFloatParam.set(5); //The .set() function set the parameter to this value.
	//myColorParam.set([1,.5,1,1]);	//for a color parameter, you need to pass an array with 3 (RGB) or 4 (RGBA) values.
	//myP2DParam.set([1.5,-5]); // for a Point2D parameter, you need to pass 2 values (XY)
	//myP3DParam.set([1.5,2,-3]); // for a Point3D parameter, you need to pass 3 values (XYZ)

	// Init average energy array
	for(var i=0;i<sample.get();i++)
	{
		energyHistory[i] = 0;
	}
	energyHistoryIndex = 0;
	actualAverageEnergy = 0;

	beatTimeStamp = util.getTime();
	beatCount = 0;
}

/*
 This function will be called each time a parameter of your script has changed
*/
function scriptParameterChanged(param)
{
	//You can use the script.log() function to show an information inside the logger panel. To be able to actuallt see it in the logger panel, you will have to turn on "Log" on this script.
	/*script.log("Parameter changed : "+param.name); //All parameters have "name" property
	if(param.is(myTrigger)) script.log("Trigger !"); //You can check if two variables are the reference to the same parameter or object with the method .is()
	else if(param.is(myEnumParam)) script.log("Label = "+param.get()+", data = "+param.getData()); //The enum parameter has a special function getData() to get the data associated to the option
	else script.log("Value is "+param.get()); //All parameters have a get() method that will return their value*/
}

/*
 This function, if you declare it, will launch a timer at 50hz, calling this method on each tick
*/

function update(deltaTime)
{

	var analyzer = analyzerValue.getTarget().get();
	actualEnergy = analyzer * analyzer;

	energyHistory[energyHistoryIndex%sample.get()] = actualEnergy;
	energyHistoryIndex = (energyHistoryIndex + 1)%sample.get();

	ComputeAverageEnergy();
	BeatDetection();
}




/* ********** MODULE SPECIFIC SCRIPTING **********************

	The "local" variable refers to the object containing the scripts. In this case, the local variable refers to the module.
	It means that you can access any control inside this module by accessing it through its address.
	For instance, if the module has a float value named "Density", you can access it via local.values.density
	Then you can retrieve its value using local.values.density.get() and change its value using local.values.density.set()
*/

/*
 This function will be called each time a parameter of this module has changed, meaning a parameter or trigger inside the "Parameters" panel of this module
 This function only exists because the script is in a module
*/
function moduleParameterChanged(param)
{
}

/*
 This function will be called each time a value of this module has changed, meaning a parameter or trigger inside the "Values" panel of this module
 This function only exists because the script is in a module
*/
function moduleValueChanged(value)
{

}

function ComputeAverageEnergy()
{
	var sum = 0;
	for(var i=0;i<sample.get();i++)
	{
		sum += energyHistory[i];
	}

	if(sample.get() != 0)
	{
		actualAverageEnergy = sum / sample.get();
	}
	else
	{
		script.logError("Can't divide by zero");
	}
}

function BeatDetection()
{
	var peak = beatThresholdSensibility.get() * actualAverageEnergy; 

	/*script.log("actualAverageEnergy : "+ actualAverageEnergy);
	script.log("actualEnergy : "+ actualEnergy);
	script.log("-------------------------------------------");*/

	if(peak < actualEnergy && util.getTime()-beatTimeStamp > beatCountIdleTime.get())
	{
		beatTimeStamp = util.getTime();
		beatCount++;
		script.log("Beat count : "+beatCount);

		if(beatCount >= beatCountTrigger.get())
		{
			local.setValid(true);
		}
	}

	if(util.getTime()-beatTimeStamp > beatCountTriggerDuration.get())
	{
		if(beatCount != 0)
		{
			script.log("Beat count : " + 0);
		}

		beatCount = 0;
		local.setValid(false);
	}

	
}
