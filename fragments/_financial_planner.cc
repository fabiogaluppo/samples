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

//Output:
/*
	maximize:
		0.0865x1 + 0.095x2 + 0.1x3 + 0.0875x4 + 0.0925x5 + 0.09x6
	subject to:
		x1 + x2 + x3 + x4 + x5 + x6 = 750000
		x1, x2, x3, x4, x5, x6 <= 187500
		x1 + x2 + x4 + x6 >= 375000
		x2 + x3 + x5 <= 262500
		x1, x2, x3, x4, x5, x6 >= 0
	solution:
		x1 = 112500.0
		x2 = 75000.0
		x3 = 187500.0
		x4 = 187500.0
		x5 = 0.0
		x6 = 187500.0
	optimal objective value: 68887.5
*/

#include "ortools/linear_solver/linear_solver.h"
#include "ortools/linear_solver/linear_solver.pb.h"

namespace operations_research {
	int main(int argc, char** argv) {
	  MPSolver solver("Ragsdale financial planner sample", MPSolver::GLOP_LINEAR_PROGRAMMING);

	  const double infinity = solver.infinity();
	  
	  //non-negative variables
	  //x1, x2, x3, x4, x5, x6 >= 0
	  MPVariable* const x1 = solver.MakeNumVar(0.0, infinity, "x1");
	  MPVariable* const x2 = solver.MakeNumVar(0.0, infinity, "x2");
	  MPVariable* const x3 = solver.MakeNumVar(0.0, infinity, "x3");
	  MPVariable* const x4 = solver.MakeNumVar(0.0, infinity, "x4");
	  MPVariable* const x5 = solver.MakeNumVar(0.0, infinity, "x5");
	  MPVariable* const x6 = solver.MakeNumVar(0.0, infinity, "x6");

	  printf("maximize:\n");

	  //objective function
	  //maximize: 0.0865x1 + 0.095x2 + 0.1x3 + 0.0875x4 + 0.0925x5 + 0.09x6 
	  printf("\t0.0865x1 + 0.095x2 + 0.1x3 + 0.0875x4 + 0.0925x5 + 0.09x6\n");
	  MPObjective* const objective = solver.MutableObjective();
	  objective->SetCoefficient(x1, 0.0865);
	  objective->SetCoefficient(x2, 0.095);
	  objective->SetCoefficient(x3, 0.1);
	  objective->SetCoefficient(x4, 0.0875);
	  objective->SetCoefficient(x5, 0.0925);
	  objective->SetCoefficient(x6, 0.09);
	  objective->SetMaximization();

	  printf("subject to:\n");

	  //1st constraint
	  //x1 + x2 + x3 + x4 + x5 + x6 = 750000
	  printf("\tx1 + x2 + x3 + x4 + x5 + x6 = 750000\n");
	  MPConstraint* const c0 = solver.MakeRowConstraint(750000.0, 750000.0);
	  c0->SetCoefficient(x1, 1);
	  c0->SetCoefficient(x2, 1);
	  c0->SetCoefficient(x3, 1);
	  c0->SetCoefficient(x4, 1);
	  c0->SetCoefficient(x5, 1);
	  c0->SetCoefficient(x6, 1);

	  //2nd constraint
	  //x1, x2, x3, x4, x5, x6 <= 187500
	  printf("\tx1, x2, x3, x4, x5, x6 <= 187500\n");
	  MPConstraint* const c1 = solver.MakeRowConstraint(0.0, 187500.0);
	  c1->SetCoefficient(x1, 1);
	  MPConstraint* const c2 = solver.MakeRowConstraint(0.0, 187500.0);
	  c2->SetCoefficient(x2, 1);
	  MPConstraint* const c3 = solver.MakeRowConstraint(0.0, 187500.0);
	  c3->SetCoefficient(x3, 1);
	  MPConstraint* const c4 = solver.MakeRowConstraint(0.0, 187500.0);
	  c4->SetCoefficient(x4, 1);
	  MPConstraint* const c5 = solver.MakeRowConstraint(0.0, 187500.0);
	  c5->SetCoefficient(x5, 1);
	  MPConstraint* const c6 = solver.MakeRowConstraint(0.0, 187500.0);
	  c6->SetCoefficient(x6, 1);

	  //3rd constraint
	  //x1 + x2 + x4 + x6 >= 375000
	  printf("\tx1 + x2 + x4 + x6 >= 375000\n");
	  MPConstraint* const c7 = solver.MakeRowConstraint(375000.0, infinity);
	  c7->SetCoefficient(x1, 1);
	  c7->SetCoefficient(x2, 1);
	  c7->SetCoefficient(x4, 1);
	  c7->SetCoefficient(x6, 1);

	  //4th constraint
	  //x2 + x3 + x5 <= 262500
	  printf("\tx2 + x3 + x5 <= 262500\n");
	  MPConstraint* const c8 = solver.MakeRowConstraint(-infinity, 262500.0);
	  c8->SetCoefficient(x2, 1);
	  c8->SetCoefficient(x3, 1);
	  c8->SetCoefficient(x5, 1);

	  printf("\tx1, x2, x3, x4, x5, x6 >= 0");

	  //solve
	  //printf("\nnumber of variables = %d", solver.NumVariables());
	  //printf("\nnumber of constraints = %d", solver.NumConstraints());
	  solver.Solve();

	  //solution
	  printf("\nsolution:");
	  printf("\n\tx1 = %.1f", x1->solution_value());
	  printf("\n\tx2 = %.1f", x2->solution_value());
	  printf("\n\tx3 = %.1f", x3->solution_value());
	  printf("\n\tx4 = %.1f", x4->solution_value());
	  printf("\n\tx5 = %.1f", x5->solution_value());
	  printf("\n\tx6 = %.1f", x6->solution_value());

	  printf("\noptimal objective value: %.1f\n", objective->Value());
	  
	  return 0;
	}
};

int main(int argc, char** argv) {
	return operations_research::main(argc, argv);
}
