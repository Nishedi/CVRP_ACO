import pandas as pd
import matplotlib.pyplot as plt

# Wczytaj dane
df = pd.read_csv("parameter_reward_log.csv")

# Lista parametrów do analizy
params = ["Alpha", "Beta", "Rho", "Q"]

# Tworzenie 4 wykresów scatter
for param in params:
    plt.figure(figsize=(8, 4))
    plt.scatter(df[param], df["Reward"], alpha=0.6)
    plt.xlabel(param)
    plt.ylabel("Reward (Optimal / Cost)")
    plt.title(f"Reward vs {param}")
    plt.grid(True)
    plt.tight_layout()
    plt.show()
