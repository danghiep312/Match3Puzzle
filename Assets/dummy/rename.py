import os
import re

dirs = os.listdir('.')
level = []

for file in dirs:
    a = re.findall(r'\d+', file)
    if (len(a) > 0):
        level.append(int(a[0]))

level.sort()
print(level)

for i in range(1, len(level) + 1):
    print(i, end=" ")
    os.rename('level' + str(level[i - 1]) + '.bytes', 'level' + str(i) + '.bytes')

