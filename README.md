# FotNET
FotNET is a simple library for working with **CONVOLUTION NEURON NETWORKS** and **CLASSIC NEURON** Networks like **PERCEPTRON**.
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

		Network netwok = new Network(layers, new LeakyReLU());

1.1. Also, before we start, u need to choose a model of neuron network. U can do this by creating a List of Layers:

		List<ILayer> layers = new List<ILayer> {
			new ConvolutionLayer();
			new ActivationLayer();
			new PoolingLayer();
			new ConvolutionLayer();
			new ActivationLayer();
			new PoolingLayer();
			new FlattenLayer();
			new PerceptronLayer();
			new ActivationLayer();
			new PerceptronLayer();
			new ActivationLayer();
			new SoftMaxLayer();
		}

1.1.1. Every layer needs a parametrs, that u should choose by ur self:

		new ConvolutionLayer(filterCount, filterHeight, filterWeight, filterDepth, convolutionStride, learningRate);
		new new ActivationLayer(activateFunction);
		new PoolingLayer(poolingSize);
		new PerceptronLayer(size, sizeOfNextLayer, learningRate);
		new PerceptronLayer(size, learningRate)

1.2. After it u should choose one of **ACTIVATION FUNCTIONS**:

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

		network.BackPropagation(expectedClass) // Index of expected class

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
**EXAMPLE**

		data = globalData; // weights that converted into string
		foreach (var layer in layers) {
			data = layer.LoadData(data);
		}

U can check that all weights are loaded correctly by checking end length of data. If length equals zero - loading was correct.
