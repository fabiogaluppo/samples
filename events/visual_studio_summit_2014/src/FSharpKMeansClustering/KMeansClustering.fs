//Sample provided by Fabio Galuppo
//April 2014

//Adaptation of http://msdn.microsoft.com/en-us/magazine/jj891054.aspx

open System

let showMatrix (matrix:double[,]) =    
    for i = 0 to Array2D.length1 matrix - 1 do
        Console.Write("[" + i.ToString().PadLeft(2) + "]  ")
        for j = 0 to Array2D.length2 matrix - 1 do
            Console.Write(matrix.[i, j].ToString("F1") + "  ");
        Console.WriteLine()        
    Console.WriteLine()

let showVector (vector:double[]) =
    for i = 0 to Array.length vector do
        Console.Write("{0} ", vector.[i].ToString("F1"))
    Console.WriteLine()
    Console.WriteLine()

let showVector_int (vector:int[]) =
    for i = 0 to Array.length vector - 1 do
        Console.Write("{0} ", vector.[i])
    Console.WriteLine()
    Console.WriteLine()

let showClustering (rawData:double[,], numClusters:int, clustering:int[]) =
    Console.WriteLine("-----------------");
    for i = 0 to numClusters - 1 do
        for j = 0 to Array2D.length1 rawData - 1 do
            if clustering.[j] = i then
                Console.Write("[" + j.ToString().PadLeft(2) + "]")
                for k = 0 to Array2D.length2 rawData - 1 do
                    Console.Write("{0} ", rawData.[j, k].ToString("F1").PadLeft(6));
                Console.WriteLine()
        Console.WriteLine("-----------------");
    Console.WriteLine()

let getData =
    let rawData = Array2D.zeroCreate<double> 20 2
    //rawData.[0, 0..1] <- [| 65.0; 220.0 |]
    //rawData.[1, 0..1] <- [| 73.0; 160.0 |]
    //...
    let values = seq[65.0, 220.0; 73.0, 160.0; 59.0, 110.0; 61.0, 120.0; 75.0, 150.0; 
                     67.0, 240.0; 68.0, 230.0; 70.0, 220.0; 62.0, 130.0; 66.0, 210.0; 
                     77.0, 190.0; 75.0, 180.0; 74.0, 170.0; 70.0, 210.0; 61.0, 110.0; 
                     58.0, 100.0; 66.0, 230.0; 59.0, 120.0; 68.0, 210.0; 61.0, 130.0]

    values |> Seq.iteri (fun i x -> rawData.[i, 0..1] <- [|fst x; snd x|])
    rawData

let initClustering (numTuples:int, numClusters:int, randomSeed:int) =
    let random = new Random(randomSeed)
    let clustering = Array.zeroCreate numTuples

    for i = 0 to numClusters - 1 do 
        clustering.[i] <- i
    
    for i = numClusters - 1 to numTuples - 1 do 
        clustering.[i] <- random.Next(0, numClusters)

    clustering
    
let allocate (numClusters:int, numAttributes:int) =
     Array2D.zeroCreate<double> numClusters numAttributes

let updateMeans (rawData:double[,], clustering:int[], means:double[,]) =
     let numClusters = Array2D.length1 means
     means |> Array2D.iteri (fun i j _ -> means.[i, j] <- 0.)
     let clusterCounts = Array.zeroCreate<int> numClusters
     
     for i = 0 to (Array2D.length1 rawData) - 1 do
        let cluster = clustering.[i]
        clusterCounts.[cluster] <- clusterCounts.[cluster] + 1
        //rawData |> Array2D.iteri (fun i j x -> means.[cluster, j] <- x)
        for j = 0 to Array2D.length2 rawData - 1 do
           means.[cluster, j] <- means.[cluster, j] + rawData.[i, j]

     means |> Array2D.iteri (fun i j x -> means.[i,j] <- x / double clusterCounts.[i])

let distance(tuple:double[], vector:double[]) =
    let sumSquaredDiffs = ref 0.
    tuple |> Array.iteri(fun i x -> sumSquaredDiffs := !sumSquaredDiffs + Math.Pow(x - vector.[i], 2.))
    Math.Sqrt(!sumSquaredDiffs)
//    let mutable sumSquaredDiffs = 0.
//    for i = 0 to Array.length tuple - 1 do
//        sumSquaredDiffs <- sumSquaredDiffs + Math.Pow(tuple.[i] - vector.[i], 2.)
//    Math.Sqrt(sumSquaredDiffs)

let computeCentroid (rawData:double[,], clustering:int[], cluster:int, means:double[,]) =
    let numAttributes = Array2D.length2 means
    let centroid = Array.zeroCreate<double> numAttributes
    let mutable minDist = Double.MaxValue
    for i = 0 to Array2D.length1 rawData - 1 do
        let c = clustering.[i]
        if c = cluster then
            let currDist = distance(rawData.[i, 0..], means.[cluster, 0..])
            if currDist < minDist then
                minDist <- currDist
                for j = 0 to Array.length centroid - 1 do
                    centroid.[j] <- rawData.[i, j]
    centroid

let updateCentroids (rawData:double[,], clustering:int[], means:double[,], centroids:double[,]) =
    for i = 0 to Array2D.length1 centroids - 1 do
        let centroid = computeCentroid(rawData, clustering, i, means)
        centroids.[i, 0..] <- centroid        

let minIndex (distances:double[]) =
    let indexOfMin = ref 0
    let smallDist = ref distances.[0]
    distances |> Array.iteri (fun i x -> if x < !smallDist then smallDist := x; indexOfMin := i)
    !indexOfMin
//    let mutable indexOfMin = 0
//    let mutable smallDist = distances.[0]
//    for i = 0 to Array.length distances - 1 do
//        if distances.[i] < smallDist then
//            smallDist <- distances.[i]
//            indexOfMin <- i
//    indexOfMin

let assign (rawData:double[,], clustering:int[], centroids:double[,]) =
    let numClusters = Array2D.length1 centroids
    let mutable changed = false

    let distances = Array.zeroCreate<double> numClusters
    for i = 0 to Array2D.length1 rawData - 1 do
        for j = 0 to numClusters - 1 do
            distances.[j] <- distance(rawData.[i, 0..], centroids.[j, 0..])
        
        let newCluster = minIndex(distances)
        if newCluster <> clustering.[i] then
            changed <- true
            clustering.[i] <- newCluster

    changed

let cluster (rawData:double[,], numClusters:int, numAttributes:int, maxCount:int) =
    let mutable changed = true
    let mutable ct = 0

    let numTuples = Array2D.length1 rawData
    
    let seed = 0 //DateTime.Now.Millisecond
    let clustering = initClustering (numTuples, numClusters, seed)
    let means = allocate (numClusters, numAttributes)
    let centroids = allocate (numClusters, numAttributes)
    updateMeans (rawData, clustering, means)
    updateCentroids (rawData, clustering, means, centroids)

    /////
    //showVector_int(clustering)
    /////

    let mutable continueLooping = changed &&  ct < maxCount
    while continueLooping do
        ct <- ct + 1
        changed <- assign (rawData, clustering, centroids)
        updateMeans (rawData, clustering, means)
        updateCentroids (rawData, clustering, means, centroids)
        
        /////
        //showVector_int(clustering)
        /////

        continueLooping <- changed &&  ct < maxCount

    clustering

[<EntryPoint>]
let main argv =
    Console.Title <- "Clustering using k-means by Fabio Galuppo (fabiogaluppo.com)" 

    Console.WriteLine("\nBegin clustering demo using k-means\n");
    Console.WriteLine("Loading all (height-weight) data into memory")

    let rawData = getData
    
    Console.WriteLine("\nRaw data:\n");
    rawData |> showMatrix

    let numAttributes = 2
    let numClusters = 3 //3
    let maxCount = 30

    Console.WriteLine("\nBegin clustering data with k = {0} and maxCount = {1}", numClusters, maxCount)
    let clustering = cluster (rawData, numClusters, numAttributes, maxCount)
    Console.WriteLine("\nClustering complete")

    Console.WriteLine("\nClustering in internal format: \n")
    clustering |> showVector_int
    
    Console.WriteLine("\nClustered data:")
    showClustering (rawData, numClusters, clustering)

    Console.WriteLine("\nEnd demo\n")
    //Console.ReadLine() |> ignore

    0