# Lock-Step Simulation in Modifiable Virtual Environments

QuantumCraft is a prototype implementation made for our research into Lock-Step simulation in Modifiable Virtual Environments (MVEs).

## Project Structure

- **quantum_code**: Contains the Visual Studio Solution and related codebase.
- **quantum_unity**: The main Unity project where the simulation is built and tested.

## Getting Started

To get started with this project, follow the steps below:

### Prerequisites

- **Git**: For cloning the repository.
- **Visual Studio**: Required to build the solution in `quantum_code`.
- **Unity 2022.3.22f1**: To build and run the simulation. The project has been tested in this version of Unity on both Linux and Windows environments.

### Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/Diar03/QuantumCraft.git
2. **Build the Visual Studio Solution**:
Navigate to the quantum_code directory and open the solution file in Visual Studio. Build the solution to compile the necessary code.
3. **Generate Asset Resources**:
After building the Visual Studio solution, you may need to import any required assets for the project. Generate the Quantum Asset resources by navigating to `Quantum->Generate Asset Resources`.
4. **Build the Project in Unity**:
Open the Unity project in Unity 2022.3.22f1.

- For **Windows**, you can compile the project using either **IL2CPP** or **Mono**.
- For **Linux**, compile the project using **Mono**.

## Usage

Once the project is built, you can run simulations to test various aspects of Lock-Step simulation in MVEs. The primary goal is to analyze the effects of different factors, such as latency and player count, on the consistency and performance of the simulation.
The maximum player count per room can be changed in `GameSetup.qtn`. Always compile the solution after making any changes in quantum_code. 
Deterministic simulation configuration can be changed in the `DeterministicConfig` asset.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
