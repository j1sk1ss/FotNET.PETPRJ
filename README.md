# FotNET
FotNET is a simple library for working with **CONVOLUTION NEURON NETWORKS** and **CLASSIC NEURON NETWORKS** like **PERCEPTRON**.
The main future is that u can create ur own neuron network without libraries that takes all work. This project is open source and u can see a code, understand him.

### Introduction:

------------

#### Importing:

First of all u should download this library and includ it into ur own project. U can do this with **NUGET** or **MANUALY** by downloading source code.

> Using FotNET;

> Using FotNET.NETWORK;

------------

#### Creation:

For creating neuron network class u need do **SIMPLE** steps:
1. Create neuron network object:

		Network netwok = new Network(layers, learningRate);

1.1. Also, before we start, u need to choose a model of neuron network. U can do this by creating a List of Layers:

		List<ILayer> layers = new List<ILayer> {
			new ConvolutionLayer(); // Input tensor get convolved by filters
			new ActivationLayer(); // Input tensor get activated
			new PoolingLayer(); // Input tensor get pooled
			new ConvolutionLayer();
			new ActivationLayer();
			new PoolingLayer();
			new FlattenLayer(); // Input tensor get converted to 1d tensor
			new PerceptronLayer(); // Input 1d tensor multiply with weights 
			new ActivationLayer();
			new PerceptronLayer();
			new ActivationLayer();
			new SoftMaxLayer(); 
		}

1.1.1. Every layer needs a parametrs, that u should choose by ur self:

		new ConvolutionLayer(filterCount, filterHeight, filterWeight, filterDepth, convolutionStride);
		new new ActivationLayer(activateFunction);
		new PoolingLayer(poolingType, poolingSize);
		new PerceptronLayer(size, sizeOfNextLayer);
		new PerceptronLayer(size)
		
1.1.1.1. Types of pooling u can find here:

		new MaxPooling();
		new MinPooling();
		new AveragePooling();

1.2. After it u should choose one of **ACTIVATION FUNCTIONS** or create ur own, but dont forget add IFunction interface:

		new ReLU();
		new LeakyReLU();
		new Sigmoid();

------------

#### ForwardFeed:

		network.ForwardFeed(image); // Put image or any tensor. 

Neuron network after **FORWARD FEED** return a **INDEX** of a predicted class from classes that u add on last **PERCEPTRON LAYER**. 

If predicted class is wrong, we going to Back Propagation.

------------

#### BackPropagation:

		network.BackPropagation(expectedClass); // Index of expected class

If expected class is different that was predicted, we should use **BACKPROPAGATION** method.

------------

#### Save and load weights:

#### Save:

All weights can be saved by getting them from every layer. 

**EXAMPLE:**

		data = "";
		foreach (var layer in layers) {
			data += layer.GetData();
		}

#### Load:

For loading weights u should use same alghoritm.

**EXAMPLE:**

		data = globalData; // weights that converted into string
		foreach (var layer in layers) {
			data = layer.LoadData(data);
		}

U can check that all weights are loaded correctly by checking end length of data. If length equals zero - loading was correct.

------------

#### Fitting and Testing

##### Fitting

Network class can be fitted and tested, and all what u need is a data set (list of data objects) and count of epochs (for fitting). Lets start on creation of data set.

		network.Fit(dataType, csvPath, csvConfig, epochCount, baseLearningRate);
		// dataType  -> Array or Tensor image
		// csvPath   -> path to csv file 
		// csvConfig -> rule that includes how should be processed csv file
		/*
		DataConfig csvConfig = new DataConfig() {
			StartRow          = 1, 
			InputColumnStart  = 1,
			InputColumnEnd    = 10,
			OutputColumnStart = 12,
			OutputColumnEnd   = 16,
			Delimiters        = new[] {";"}
		}
		*/
		// epochCount -> count of epochs
		// baseLearningRate -> start learning rate 

This is a implementaion of data set. "new Image()" as u can see - implementaion of image. First part where we can see "new double[,,]" very simple and means x,y and depth. Second part is answer for this data set unit. For example we should choose one of ten numbers, and we should create an array where index of needed element have vale equals 1. 

##### Testing

After fitting u can test ur own model by using next method with test data set:

		double accuracy = network.Test(dataType, csvPath, csvConfig);

