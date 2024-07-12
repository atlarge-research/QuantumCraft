import psutil
import subprocess
import time
import argparse
from concurrent.futures import ThreadPoolExecutor
import csv

def run_and_monitor(process):
    network_stats = []

    try:
        while process.poll() is None:  # Check if process is still running
            p = psutil.Process(process.pid)
            io_counters = p.net_io_counters()
            network_stats.append([time.time(), io_counters.bytes_sent, io_counters.bytes_recv])
            time.sleep(1)
    except psutil.NoSuchProcess:
        pass

    return process.pid, network_stats

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Run and monitor game instances.")
    parser.add_argument("num_instances", type=int, help="Number of game instances to run")
    parser.add_argument("duration", type=int, help="Duration to run the instances (in seconds)")
    args = parser.parse_args()

    processes = []
    for _ in range(args.num_instances):
        processes.append(subprocess.Popen(["./QuantumCraft.x86_64"]))

    start_time = time.time()
    with ThreadPoolExecutor() as executor:
        results = executor.map(run_and_monitor, processes)

    end_time = time.time()

    # Terminate processes
    for process in processes:
        if process.poll() is None:
            process.terminate()
            process.wait()

    # Write network stats to CSV
    with open("bandwidth_data.csv", "w", newline="") as csvfile:
        fieldnames = ["Timestamp", "Bytes_Sent", "Bytes_Received", "PID"]
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()
        for pid, stats in results:
            for row in stats:
                writer.writerow({
                    "Timestamp": row[0],
                    "Bytes_Sent": row[1],
                    "Bytes_Received": row[2],
                    "PID": pid
                })

    print(f"Network statistics saved to bandwidth_data.csv")
    print(f"\nTotal Runtime: {end_time - start_time:.2f} seconds")
