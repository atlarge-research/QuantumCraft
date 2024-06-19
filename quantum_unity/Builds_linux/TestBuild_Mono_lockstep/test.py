import time
import csv
import subprocess
import signal
import argparse

# --- Global Variables ---
data = []
end_time = None
game_processes = []
last_net_counters = {'bytes_sent': 0, 'bytes_recv': 0}


# --- Functions ---
def launch_game_instance(instance_number):
    """Launches a single instance of the game in headless mode and returns its process."""
    process = subprocess.Popen(["./QuantumCraft.x86_64"], 
                               stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    print(f"Launched headless instance {instance_number} with PID {process.pid}")
    return process


def monitor_bandwidth(interval, duration):
    global end_time, last_net_counters

    end_time = time.time() + duration

    with open("/proc/net/dev") as f:
        lines = f.readlines()

    for line in lines:
        if "eth0" in line:  # Or your relevant interface (e.g., 'wlan0' for Wi-Fi)
            last_net_counters['bytes_recv'], last_net_counters['bytes_sent'], *_ = map(int, line.split()[1::2])
            break

    while time.time() < end_time:
        with open("/proc/net/dev") as f:
            lines = f.readlines()
        for line in lines:
            if "eth0" in line:  # Or your relevant interface
                bytes_recv, bytes_sent, *_ = map(int, line.split()[1::2])
                break

        net_sent = bytes_sent - last_net_counters['bytes_sent']
        net_recv = bytes_recv - last_net_counters['bytes_recv']
        last_net_counters['bytes_sent'] = bytes_sent
        last_net_counters['bytes_recv'] = bytes_recv

        print("Net Sent: {}, Net Recv: {}".format(net_sent, net_recv))  # Replace with your desired logging
        data.append({
            'Timestamp': time.time(), 
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
        process = launch_game_instance(i + 1)
        game_processes.append(process)
        time.sleep(2)  # Give instances time to start up

    monitor_bandwidth(args.interval, args.duration)

    # Terminate processes again in case some didn't exit cleanly
    for process in game_processes:
        process.kill()

    with open("bandwidth_data.csv", "w", newline="") as csvfile:
        fieldnames = ['Timestamp', 'Net Sent (Bytes)', 'Net Recv (Bytes)']
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()
        for row in data:
            writer.writerow(row)
