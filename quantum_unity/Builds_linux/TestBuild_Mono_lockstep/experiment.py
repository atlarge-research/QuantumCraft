import psutil
import time
import argparse
import signal
import subprocess
import csv
from concurrent.futures import ThreadPoolExecutor
from threading import Thread

# --- Constants ---
GAME_EXECUTABLE = "./QuantumCraft.x86_64"
LAUNCH_DELAY = 2
DATA_FILE = "bandwidth_data.csv"

# --- Global Data ---
data = []  # Shared data for bandwidth monitoring
end_time = None
game_processes = []


# --- Functions ---

def launch_game(instance_number):
    """Launches a single game instance in headless mode."""
    process = subprocess.Popen(
        [GAME_EXECUTABLE, "-batchmode", "-nographics"],
        stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL,
    )
    print(f"Launched headless instance {instance_number} with PID {process.pid}")
    time.sleep(LAUNCH_DELAY)
    return process


def monitor_bandwidth(interval, duration):
    """Monitors total bandwidth usage and stores data."""
    global end_time
    psutil.net_io_counters.cache_clear()  # Clear cached network counters
    end_time = time.time() + duration
    last_counters = psutil.net_io_counters()

    while time.time() < end_time:
        time.sleep(interval)
        current_counters = psutil.net_io_counters()
        net_sent = current_counters.bytes_sent - last_counters.bytes_sent
        net_recv = current_counters.bytes_recv - last_counters.bytes_recv

        data.append(
            {
                "Timestamp": time.strftime("%Y-%m-%d %H:%M:%S"),
                "Net Sent (Bytes)": net_sent,
                "Net Recv (Bytes)": net_recv,
            }
        )
        last_counters = current_counters  # Update for next iteration


def signal_handler(sig, frame):
    """Handles Ctrl+C signal to terminate game instances."""
    global end_time
    print("Terminating game instances...")
    end_time = time.time()
    for process in game_processes:
        process.terminate()


# --- Main Execution ---

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Launch and monitor Unity game instances.")
    parser.add_argument("num_instances", type=int, help="Number of instances to launch")
    parser.add_argument("interval", type=float, help="Data collection interval (seconds)")
    parser.add_argument("duration", type=float, help="Monitoring duration (seconds)")
    args = parser.parse_args()

    signal.signal(signal.SIGINT, signal_handler)

    # Launch game instances concurrently
    with ThreadPoolExecutor() as executor:
        game_processes = list(executor.map(launch_game, range(1, args.num_instances + 1)))

    # Start bandwidth monitoring
    monitor_thread = Thread(target=monitor_bandwidth, args=(args.interval, args.duration))
    monitor_thread.start()
    monitor_thread.join()  # Wait for monitoring to finish

    # Terminate game instances and save data
    for process in game_processes:
        process.terminate()
        process.wait()

    with open(DATA_FILE, "w", newline="") as csvfile:
        fieldnames = ["Timestamp", "Net Sent (Bytes)", "Net Recv (Bytes)"]
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(data)

    print(f"Data saved to {DATA_FILE}")
