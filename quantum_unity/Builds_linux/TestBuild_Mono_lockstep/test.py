import psutil
import time
import pandas as pd
import argparse
import signal
import subprocess

# --- Global Variables ---
data = []
end_time = None
game_processes = []

# --- Functions ---
def launch_game_instance(instance_number):
    """Launches a single instance of the game in headless mode and returns its process."""
    process = subprocess.Popen(["./QuantumCraft.x86_64"], #"-batchmode", "-nographics"],  
                             stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL) 
    print(f"Launched headless instance {instance_number} with PID {process.pid}")
    return process

def monitor_bandwidth(interval, duration):
    """Monitors overall bandwidth usage (not per process)."""
    global end_time
    psutil.net_io_counters.cache_clear()
    end_time = time.time() + duration
    last_net_counters = psutil.net_io_counters()

    while time.time() < end_time:
        current_net_counters = psutil.net_io_counters()
        net_sent = current_net_counters.bytes_sent - last_net_counters.bytes_sent
        net_recv = current_net_counters.bytes_recv - last_net_counters.bytes_recv
        print(current_net_counters)
        last_net_counters = current_net_counters

        data.append({
            'Timestamp': pd.Timestamp.now(),
            'Net Sent (Bytes)': net_sent,
            'Net Recv (Bytes)': net_recv
        })

        time.sleep(interval)

def signal_handler(sig, frame):
    """Handles Ctrl+C (SIGINT) signal to terminate game processes."""
    global end_time 
    print("Ctrl+C pressed. Terminating game instances...")
    for process in game_processes:
        process.terminate()
    end_time = time.time()  # Immediately stop the monitoring loop

# --- Main Script ---
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Launch and monitor Unity game instances.")
    parser.add_argument("num_instances", type=int, help="Number of game instances to launch")
    parser.add_argument("interval", type=float, help="Data collection interval in seconds")
    parser.add_argument("duration", type=float, help="Total monitoring duration in seconds")
    args = parser.parse_args()

    signal.signal(signal.SIGINT, signal_handler)

    for i in range(args.num_instances):
        #process = launch_game_instance(i + 1)
        #game_processes.append(process)
        time.sleep(2) 

    monitor_bandwidth(args.interval, args.duration)

    # Terminate processes again in case some didn't exit cleanly
    #for process in game_processes:
    #    process.terminate()
    #    process.wait()

    df = pd.DataFrame(data)
    df.to_csv("bandwidth_data.csv", index=False)