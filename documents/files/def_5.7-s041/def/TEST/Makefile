FAKE_ALL: all

all: test

install: test

dotest: test

clean doclean:
	rm -f run.output run.stderr diff.out

TEST_FILE = complete.5.7.def

TEST_GOLD = complete.5.7.def.au

test: ../bin/defrw ../bin/defdiff
	../bin/defrw $(TEST_FILE) > run.output 2> run.stderr
	diff run.output $(TEST_GOLD)
