![Alt Text](https://github.com/j1sk1ss/FotNET.MPRJ/blob/master/Cover.png)
# FotNET
FotNET is a simple library for working with **REGIONAL-CONVOLUTION**, **RECURRENT**, **GENERATIVE-ADVERSARIAL**, **CONVOLUTION** and **CLASSIC** **NEURAL NETWORKS** like **PERCEPTRON**.
The main part is that u can create ur own neural network without libraries that takes all work. This project is open source and u can see a code, download him and change every part what u need cuz it very simple to understand this project with xml documentation.
![Alt Text](https://github.com/j1sk1ss/FotNET.MPRJ/blob/master/firstSlide.png)
### Introduction:

------------

#### Importing:

First of all u should download this library and includ it into ur own project. U can do this with **NUGET** or **MANUALY** by downloading source code.

> Using FotNET;

> Using FotNET.NETWORK;

------------

#### Creation:

For creating neural network class u need do **SIMPLE** steps:
1. Create neural network object:

		Network netwok = new Network(layers);

1.1. Also, before we start, u need to choose a model of neural network. U can do this by creating a List of Layers:

		List<ILayer> layers = new List<ILayer> {
			new ConvolutionLayer(); // Input tensor get convolved by filters
			new ActivationLayer(); // Input tensor get activated
			new DropoutLayer();
			new PoolingLayer(); // Input tensor get pooled
			new ConvolutionLayer();
			new ActivationLayer();
			new DropoutLayer();
			new PoolingLayer();
			new FlattenLayer(); // Input tensor get converted to 1d tensor
			new PerceptronLayer(); // Input 1d tensor multiply with weights 
			new ActivationLayer();
			new DropoutLayer();
			new PerceptronLayer();
			new ActivationLayer();
			new SoftMaxLayer(); 
		}
		
		>> OR
		
		List<ILayer> layers = new List<ILayer> {
			new FlattenLayer();
			new RecurrentLayer();
			new SoftMaxLayer(); 
		}
		
		>> OR
		
		List<ILayer> layers = new List<ILayer> {
			new RoughenLayer(); // converts 1D tensor to multi-dimensional tensor
			new DeconvolutionLayer(); // Input tensor get convolved by filters
			new ActivationLayer(); // Input tensor get activated
			new DeconvolutionLayer();
			new ActivationLayer();
			new NormalizationLayer();
			new DataLayer();
		}
		
		>> OR
		
		List<ILayer> layers = new List<ILayer> {
			new BatchNormalizationLayer(); // Batch normalizer
			new UpSampleLayer(); // Up sample input tensor by one of types of up sampling
			new ConvolutionLayer();
			new ActivationLayer(); // Input tensor get activated
			new UpSampleLayer(); 
			new ConvolutionLayer();
			new ActivationLayer(); 
			new NormalizationLayer();
			new DataLayer();
		}

1.1.1. Every layer needs a parametrs, that u should choose by ur self:

		new ConvolutionLayer(filterCount, filterHeight, filterWeight, filterDepth, weightInitialization, convolutionStride, optimization);
		// or
		new ConvolutionLayer(pathToFilter, convolutionStride); // path to filter is a path to ur custom filter. Example of custom filters u can find in the end of ReadMe.
		
		new ActivationLayer(activateFunction);
		new PoolingLayer(poolingType, poolingSize);
		new PerceptronLayer(size, sizeOfNextLayer, weightInitialization, optimization);
		new PerceptronLayer(size);
		new DropoutLayer(percentOfDropped);
		new RecurrentLayer(activationFunction, recurrencyType, hiddenLayerSize, weightInitialization);
		new TransposedConvolutionLayer(filterCount, filterHeight, filterWeight, filterDepth, weightInitialization, convolutionStride);
		new NormalizationLayer(normalizationType);
		
1.1.1.1. Types of pooling u can find here:

		new MaxPooling();
		new MinPooling();
		new AveragePooling();
		new BilinearPooling();

1.1.1.2. Variants of weight initialization:

		new HeInitialization(); // Usualy uses with ReLU, Leaky ReLU and Double Leaky ReLU
		new XavierInitialization(); // Also uses with Tanghensoid and Sighmoid
		new NormalizedXavierInitialization(); // Same with upper activation function, but normalized
		new ConstInitialization();
		new RandomInitialization();
		new ZeroInitialization();
		new LeCunNormalization();

1.1.1.3. Types of normalization:

		new Abs(); // Normalize tensor with Abs
		new MinMax(); // Normalize tensor between min and max value

1.1.1.4. Types of Up sampling

		new NearestNeighbor();
		new BilinearInterpolation();
		new BicubicInterpolation();

1.1.1.5 Types of Perceptron Optimization:

		new AdamPerceptronOptimization();
		new NoPerceptronOptimization();
		
1.1.1.6 Types of Convolution Optimization:

		new AdamConvolutionOptimization();
		new NoConvolutionOptimization();

1.2. After it u should choose one of **ACTIVATION FUNCTIONS** or create ur own, but dont forget add Function abstract class:

		new ReLu();
		new LeakyReLu();
		new DoubleLeakyReLu();
		new Sigmoid();
		new Tangensoid();
		new GeLu();
		new BinaryStep();
		new SoftPlus();
		new Identity();
		new Swish();
		new HardSigmoid();
		new SiLu();
		new SeLu();
		new HyperbolicTangent();

------------

#### ForwardFeed:

		network.ForwardFeed(tensor); // Put image or any tensor. 
		
		>> OR
		
		network.ForwardFeed(tensor, answerType); // Answer type - class or value of class

Neural network after **FORWARD FEED** depending of ur chose return a **INDEX** of a predicted class from classes that u add on last **PERCEPTRON LAYER** or **VALUE**.
Or, if u don`t add answer type option, returns tensor of answers.

If predicted class is wrong, we going to Back Propagation.

------------

#### BackPropagation:

		network.BackPropagation(expectedClass, expectedValue, lossFunction, learningRate, backPropagateStatus); // Index of expected class and a value (usualy is 1)
		
		>> OR
		
		network.BackPropagation(expectedTensor, lossFunction, learningRate, backPropagateStatus);
		
		>> OR
		
		network.BackPropagation(error, learningRate, backPropagateStatus);
		
U can see lossFunction option:

		new Mse();
		new Mae();
		new Cost();
		new CeLl();
		new Mape();
		new Mbe();
		
Also u can add Regularization param like below:

		new *LOSS_FUNC*(new L1());
		new *LOSS_FUNC*(new L2());
		new *LOSS_FUNC*(new NoRegularization());
		
If expected class is different that was predicted, we should use **BACKPROPAGATION** method.

------------

#### Save and load weights:

#### Save:

All weights can be saved by using next method. 

**EXAMPLE:**

		data = network.GetWeights();

#### Load:

For loading weights u should use same alghoritm.

**EXAMPLE:**

		network.LoadWeights(data);

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

------------

### Examples

#### Custom filters

U should create a custom file.txt, after fill it like:

	1 0 1
	1 0 1
	1 0 1

If u want add few filters, add other constructions and separate them by '//n\':

	1 0 1
	1 0 1
	1 0 1/
	0 0 0
	1 1 1
	0 0 0/
	1 0 0
	0 1 0
	0 0 1
