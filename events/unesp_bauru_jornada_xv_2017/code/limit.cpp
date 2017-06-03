//Sample provided by Fabio Galuppo  
//June 2017

//compile: 
//g++ -pipe -Wall -O2 -std=c++14 limit.cpp

//compile: 
//clang++ -Wall -O2 -std=c++14 limit.cpp

#include <functional>
#include <cmath>

bool limit (std::function<float(float)> f, float x0, float* result) {
    const float threshold = 0.0001f;
    auto left =  f (x0 - threshold); 
    auto right = f (x0 + threshold);
    auto estimate = (left + right) / 2.f;
    if (std::abs(left - right) < 0.01f) {
        *result = estimate;
        return true;
    }
    return false;
}

#include <cstdio>

void display_limit(std::function<float(float)> f, float x0, const char* label = nullptr) {
    if (label)
        printf("f: %s\n", label);
    float result;
    if (limit(f, x0, &result)) {
        printf("The limit of f when x approaches %f is %f\n\n", x0, result);
    } else {
        printf("The limit does not exist\n\n");
    }
}

int main() {
    display_limit([](float x){ return 3.f * std::pow(x, 2.f); }, 2.f, "lim(3 * x^2), x->2");

    display_limit([](float x){ return (2.f * x + 1.f) / std::pow(x, 2.f); }, 3.f, "lim((2 * x + 1) / (x^2)), x->3");

    display_limit([](float x){ return (std::pow(x, 2.f) - 1.f) / (x - 1.f); }, 1.f, "lim((x^2 - 1) / (x - 1)), x->1");

    display_limit([](float x){ return std::sin(5.f / (x - 1.f)); }, 1.f, "lim(sin(5 / (x - 1))), x->1");

    display_limit([](float x){ return std::exp(x); }, 0.f, "lim(e^x), x->0");

    display_limit([](float x){ return std::sin(x) / x; }, 0.f, "lim(sin(x) / x), x->0");

    return 0;
}