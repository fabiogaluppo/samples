//Sample provided by Fabio Galuppo
//March 2018

//Makefile.user:
/*
$(OBJ_DIR)$S_financial_planner.$O: $(EX) $(CVRPTW_DEPS) $(DIMACS_DEPS) $(FAP_DEPS)
    $(CCC) $(CFLAGS) -c $(EX) $(OBJ_OUT)$(OBJ_DIR)$S$(basename $(notdir $(EX))).$O

$(BIN_DIR)/_financial_planner$E: $(OR_TOOLS_LIBS) $(CVRPTW_LIBS) $(DIMACS_LIBS) $(FAP_LIBS) \
    $(OBJ_DIR)$S_financial_planner.$O
    $(CCC) $(CFLAGS) $(OBJ_DIR)$S_financial_planner.$O $(OR_TOOLS_LNK) $(CVRPTW_LNK) $(DIMACS_LNK) \
    $(FAP_LNK) $(OR_TOOLS_LD_FLAGS) $(EXE_OUT)$(BIN_DIR)$S_financial_planner$E
*/

//run sample:
//make rcc EX=_financial_planner.cc

//portfolio:
/*
    ----------------------------------------------------------
    |            | expected    |        |                    |
    |investiment | year return | risk   | kind               |
    |--------------------------------------------------------|
    |     x1     |   8%        | low    | fixed income type 1|
    |     x2     |   7%        | low    | fixed income type 2|
    |     x3     |  12%        | medium | shares type 1      |
    |     x4     |  10.5%      | medium | foreign exchange   |
    |     x5     |  16.5%      | high   | shares type 2      |
    |     x6     |  80%        | high   | cryptocurrency     |
    ----------------------------------------------------------
*/

//output:
/*
    ammount to invest? 100000
    investment risk profile 1)conservative 2)moderate 3)aggressive? 2
    high risk max [20%, 60%]? 35
    maximize:
        0.08x1 + 0.07x2 + 0.12x3 + 0.105x4 + 0.165x5 + 0.8x6
    subject to:
        x1 + x2 + x3 + x4 + x5 + x6 = 100% of ammount
        x3 + x4 >= 30% of ammount
        x5 + x6 <= 35% of ammount
        x4 >= 15% of ammount
        x1 + x2 >= 15% of ammount
        x6 <= 10% of ammount
        x1, x2, x3, x4, x5, x6 >= 5% of ammount
    solution:
        x1 =  10000.0 (10.0%)
        x2 =   5000.0 ( 5.0%)
        x3 =  35000.0 (35.0%)
        x4 =  15000.0 (15.0%)
        x5 =  25000.0 (25.0%)
        x6 =  10000.0 (10.0%)
    optimal expected year rate of return: 19050.0   
*/

#include "ortools/linear_solver/linear_solver.h"
#include "ortools/linear_solver/linear_solver.pb.h"
#include <iostream>

namespace operations_research {
    void conservative(MPSolver& solver, double ammount, double medium_risk_max,
        MPVariable* const x1, MPVariable* const x2, 
        MPVariable* const x3, MPVariable* const x4,
        MPVariable* const x5, MPVariable* const x6) {

        const double infinity = solver.infinity();

        printf("\tx3 + x4 <= %.0f%% of ammount\n", medium_risk_max * 100);
        MPConstraint* const c1 = solver.MakeRowConstraint(0.0, medium_risk_max * ammount);
        c1->SetCoefficient(x3, 1);
        c1->SetCoefficient(x4, 1);
        
        printf("\tx4 >= 10%% of ammount\n");
        MPConstraint* const c2 = solver.MakeRowConstraint(0.1 * ammount, infinity);
        c2->SetCoefficient(x4, 1);

        printf("\t5x1 + 5x2 - x4 >= 0\n");
        MPConstraint* const c3 = solver.MakeRowConstraint(0.0, infinity);
        c3->SetCoefficient(x1, 5);
        c3->SetCoefficient(x2, 5);
        c3->SetCoefficient(x4, -1);

        printf("\tx5, x6 = 0\n");
        MPConstraint* const c4 = solver.MakeRowConstraint(0.0, 0.0);
        c4->SetCoefficient(x5, 1);
        MPConstraint* const c5 = solver.MakeRowConstraint(0.0, 0.0);
        c5->SetCoefficient(x6, 1);

        printf("\tx1 <= 65%% of ammount\n");
        MPConstraint* const c6 = solver.MakeRowConstraint(0.0, 0.65 * ammount);
        c6->SetCoefficient(x1, 1);
    }

    void balanced(MPSolver& solver, double ammount, double high_risk_max,
        MPVariable* const x1, MPVariable* const x2, 
        MPVariable* const x3, MPVariable* const x4,
        MPVariable* const x5, MPVariable* const x6) {

        const double infinity = solver.infinity();

        printf("\tx3 + x4 >= 30%% of ammount\n");
        MPConstraint* const c1 = solver.MakeRowConstraint(0.3 * ammount, infinity);
        c1->SetCoefficient(x3, 1);
        c1->SetCoefficient(x4, 1);

        printf("\tx5 + x6 <= %.0f%% of ammount\n", high_risk_max * 100);
        MPConstraint* const c2 = solver.MakeRowConstraint(0.0, high_risk_max * ammount);
        c2->SetCoefficient(x5, 1);
        c2->SetCoefficient(x6, 1);

        printf("\tx4 >= 15%% of ammount\n");
        MPConstraint* const c3 = solver.MakeRowConstraint(0.15 * ammount, infinity);
        c3->SetCoefficient(x4, 1);

        printf("\tx1 + x2 >= 15%% of ammount\n");
        MPConstraint* const c4 = solver.MakeRowConstraint(0.15 * ammount, infinity);
        c4->SetCoefficient(x1, 1);
        c4->SetCoefficient(x2, 1);

        printf("\tx6 <= 10%% of ammount\n");
        MPConstraint* const c5 = solver.MakeRowConstraint(0.0, 0.1 * ammount);
        c5->SetCoefficient(x6, 1);

        printf("\tx1, x2, x3, x4, x5, x6 >= 5%% of ammount\n");
        MPConstraint* const c6 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c6->SetCoefficient(x1, 1);
        MPConstraint* const c7 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c7->SetCoefficient(x2, 1);
        MPConstraint* const c8 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c8->SetCoefficient(x3, 1);
        MPConstraint* const c9 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c9->SetCoefficient(x4, 1);
        MPConstraint* const c10 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c10->SetCoefficient(x5, 1);
        MPConstraint* const c11 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c11->SetCoefficient(x6, 1);
    }

    void aggressive(MPSolver& solver, double ammount, double high_risk_max,
        MPVariable* const x1, MPVariable* const x2, 
        MPVariable* const x3, MPVariable* const x4,
        MPVariable* const x5, MPVariable* const x6) {

        const double infinity = solver.infinity();

        printf("\tx5 + x6 <= %.0f%% of ammount\n", high_risk_max * 100);
        MPConstraint* const c1 = solver.MakeRowConstraint(0.0, high_risk_max * ammount);
        c1->SetCoefficient(x5, 1);
        c1->SetCoefficient(x6, 1);

        printf("\tx4 >= 25%% of ammount\n");
        MPConstraint* const c2 = solver.MakeRowConstraint(0.25 * ammount, infinity);
        c2->SetCoefficient(x4, 1);

        printf("\t10x3+ 10x4 - x1 - x2 >= 0\n");
        MPConstraint* const c3 = solver.MakeRowConstraint(0.0, infinity);
        c3->SetCoefficient(x3, 10);
        c3->SetCoefficient(x4, 10);
        c3->SetCoefficient(x1, -1);
        c3->SetCoefficient(x2, -1);

        printf("\tx4 >= 25%% of ammount\n");
        MPConstraint* const c4 = solver.MakeRowConstraint(0.25 * ammount, infinity);
        c4->SetCoefficient(x3, 1);
        c4->SetCoefficient(x5, 1);

        printf("\tx1, x2, x3 >= 5%% of ammount\n");
        MPConstraint* const c5 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c5->SetCoefficient(x1, 1);
        MPConstraint* const c6 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c6->SetCoefficient(x2, 1);
        MPConstraint* const c7 = solver.MakeRowConstraint(0.05 * ammount, infinity);
        c7->SetCoefficient(x3, 1);
    }

    int main(int argc, char** argv) {
        double ammount;
        printf("ammount to invest? ");
        std::cin >> ammount;
        int risk_profile;
        printf("investment risk profile 1)conservative 2)moderate 3)aggressive? ");
        std::cin >> risk_profile;
        if (ammount < 1000.0 && !(1 <= risk_profile && risk_profile <= 3)) {
            printf("not accepted\n");
            return -1;
        }

        int risk_max;
        switch(risk_profile) {
            case 1:
                printf("medium risk max [15%%, 35%%]? ");
                std::cin >> risk_max;
                if (!(15 <= risk_max && risk_max <= 35)) {
                    printf("not accepted\n");
                    return -1;
                }
                break;
            case 2:
                printf("high risk max [20%%, 60%%]? ");
                std::cin >> risk_max;
                if (!(20 <= risk_max && risk_max <= 60)) {
                    printf("not accepted\n");
                    return -1;
                }
                break;
            case 3:
            default:
                printf("high risk max [50%%, 95%%]? ");
                std::cin >> risk_max;
                if (!(50 <= risk_max && risk_max <= 95)) {
                    printf("not accepted\n");
                    return -1;
                }
                break;
        }

        MPSolver solver("Financial planner", MPSolver::GLOP_LINEAR_PROGRAMMING);

        const double infinity = solver.infinity();

        //non-negative variables
        MPVariable* const x1 = solver.MakeNumVar(0.0, infinity, "x1");
        MPVariable* const x2 = solver.MakeNumVar(0.0, infinity, "x2");
        MPVariable* const x3 = solver.MakeNumVar(0.0, infinity, "x3");
        MPVariable* const x4 = solver.MakeNumVar(0.0, infinity, "x4");
        MPVariable* const x5 = solver.MakeNumVar(0.0, infinity, "x5");
        MPVariable* const x6 = solver.MakeNumVar(0.0, infinity, "x6");

        printf("maximize:\n");

        //maximize objective function
        printf("\t0.08x1 + 0.07x2 + 0.12x3 + 0.105x4 + 0.165x5 + 0.8x6\n");
        MPObjective* const objective = solver.MutableObjective();
        objective->SetCoefficient(x1, 0.08);
        objective->SetCoefficient(x2, 0.07);
        objective->SetCoefficient(x3, 0.12);
        objective->SetCoefficient(x4, 0.105);
        objective->SetCoefficient(x5, 0.165);
        objective->SetCoefficient(x6, 0.8);
        objective->SetMaximization();

        printf("subject to:\n");

        //total investment
        printf("\tx1 + x2 + x3 + x4 + x5 + x6 = 100%% of ammount\n");
        MPConstraint* const c0 = solver.MakeRowConstraint(ammount, ammount);
        c0->SetCoefficient(x1, 1);
        c0->SetCoefficient(x2, 1);
        c0->SetCoefficient(x3, 1);
        c0->SetCoefficient(x4, 1);
        c0->SetCoefficient(x5, 1);
        c0->SetCoefficient(x6, 1);

        //select risk profile
        switch(risk_profile) {
            case 1:
                conservative(solver, ammount, risk_max / 100.0, x1, x2, x3, x4, x5, x6);
                break;
            case 2:
                balanced(solver, ammount, risk_max / 100.0, x1, x2, x3, x4, x5, x6);
                break;
            case 3:
            default:
                aggressive(solver, ammount, risk_max / 100.0, x1, x2, x3, x4, x5, x6);
                break;
        }
        
        //solve
        solver.Solve();

        //solution
        printf("solution:");
        printf("\n\tx1 = %8.1f (%4.1f%%)", x1->solution_value(), 100.0 * x1->solution_value() / ammount);
        printf("\n\tx2 = %8.1f (%4.1f%%)", x2->solution_value(), 100.0 * x2->solution_value() / ammount);
        printf("\n\tx3 = %8.1f (%4.1f%%)", x3->solution_value(), 100.0 * x3->solution_value() / ammount);
        printf("\n\tx4 = %8.1f (%4.1f%%)", x4->solution_value(), 100.0 * x4->solution_value() / ammount);
        printf("\n\tx5 = %8.1f (%4.1f%%)", x5->solution_value(), 100.0 * x5->solution_value() / ammount);
        printf("\n\tx6 = %8.1f (%4.1f%%)", x6->solution_value(), 100.0 * x6->solution_value() / ammount);
        printf("\noptimal expected year rate of return: %.1f\n", objective->Value());

        return 0;
    }
};

int main(int argc, char** argv) {
    return operations_research::main(argc, argv);
}