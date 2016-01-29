# -*- coding: utf-8 -*-

# inputs = ['光', '暗', '金', '木', '水', '火', '土']
# inputs = ['金', '木', '水', '火', '土']
inputs = ['光', '暗', '金', '水', '火']
outputs = []

def pick(inputs, picks, pick_count):
	if pick_count <= 0:
		outputs.append(picks)
		print ''.join(picks).decode('utf-8').encode('gbk')
		return

	for i in range(len(inputs)):
		for j in range(pick_count, 0, -1):
			ele = inputs[i]
			new_picks = picks[:] + [ele] * j
			new_inputs = inputs[i+1:]
			pick(new_inputs, new_picks, pick_count - j)

print '-----------------------------'
pick(inputs, [], 3)
print len(outputs)


