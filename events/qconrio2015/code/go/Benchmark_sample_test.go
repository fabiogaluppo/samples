//Sample provided by Fabio Galuppo 
//August 2015

//go test -v -run="none" -bench="BenchmarkMax"
//go test -v -run="none" -bench=.
//go test -v -run="none" -bench=. -benchtime="5s"

package benchmark_sample_test
import (
    //"fmt"
    "testing"
    "math"
    "math/rand"
    "time"
)

const N = 10000000

func BenchmarkMax(b *testing.B) {
    b.ResetTimer()
    
    r := rand.New(rand.NewSource(time.Now().UnixNano()))
    var a [N]float64
    for i := 0; i < N; i++ {
        a[i] = r.Float64()
    }

    max := 0.0
    for i := 0; i < N; i++ {
        max = math.Max(max, a[i]) 
    }
    //fmt.Printf("\n%f\n", max)
}

func BenchmarkMin(b *testing.B) {
    b.ResetTimer()
    
    r := rand.New(rand.NewSource(time.Now().UnixNano()))
    var a [N]float64
    for i := 0; i < N; i++ {
        a[i] = r.Float64()
    }
    
    min := 0.0
    for i := 0; i < N; i++ {
        min = math.Min(min, a[i]) 
    }
    //fmt.Printf("\n%f\n", min)
}
