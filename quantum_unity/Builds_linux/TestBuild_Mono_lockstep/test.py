import psutil
import time
import pandas as pd
import argparse
import signal
import subprocess

# --- Global Variables ---
data = []
end_time = None
game_processes = {}  

# --- Functions ---
def launch_game_instance(instance_number):
    """Launches a single instance of the game in headless mode and returns its process."""
    process = subprocess.Popen(["./QuantumCraft.x86_64"],  
                               stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL) 
    print(f"Launched headless instance {instance_number} with PID {process.pid}")
    return process

def monitor_bandwidth(interval, duration):
    """Monitors bandwidth per process"""
    global end_time, game_processes
    psutil.net_io_counters.cache_clear()  
    end_time = time.time() + duration

    while time.time() < end_time:
        for process in list(game_processes.keys()):  
            try:
                net_io = process.oneshot().children()[0].connections() 
                if net_io:
                    net_sent, net_recv = net_io[0].bytes_sent, net_io[0].bytes_recv
                    game_processes[process].append((net_sent, net_recv))  
            except (psutil.NoSuchProcess, psutil.AccessDenied, IndexError):
                del game_processes[process] 

        time.sleep(interval)

def signal_handler(sig, frame):
    """Handles Ctrl+C (SIGINT) signal to terminate game processes."""
    global end_time 
    print("Ctrl+C pressed. Terminating game instances...")
    for process in game_processes:
        process.terminate()
    end_time = time.time()  


# --- Main Script ---
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Launch and monitor Unity game instances.")
    parser.add_argument("num_instances", type=int, help="Number of game instances to launch")
    parser.add_argument("interval", type=float, help="Data collection interval in seconds")
    parser.add_argument("duration", type=float, help="Total monitoring duration in seconds")
    args = parser.parse_args()

    signal.signal(signal.SIGINT, signal_handler)

    for i in range(args.num_instances):
        process = launch_game_instance(i + 1)
        game_processes[process] = []

    monitor_bandwidth(args.interval, args.duration)

    # Data Processing
    all_data = []
    for process, stats in game_processes.items():
        for net_sent, net_recv in stats:
            all_data.append({
                'Timestamp': pd.Timestamp.now(),
                'PID': process.pid,
                'Net Sent (Bytes)': net_sent,
                'Net Recv (Bytes)': net_recv
            })

    # Terminate game processes 
    for process in game_processes:
        process.terminate()
        process.wait()

    df = pd.DataFrame(all_data)
    df.to_csv("bandwidth_data_per_process.csv", index=False)
