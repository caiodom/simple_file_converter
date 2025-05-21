#!/usr/bin/env bash
# Executa 10 processos em background, cada um 100 iters
for i in $(seq 1 10); do
  newman run ../postman_collection.json \
    --environment ../env.json \
    --iteration-count 100 \
    --delay-request 0 \
    --reporters cli,json \
    --reporter-json-export report-$i.json \
    && echo "Instance $i done" &
done
wait
echo "All load-test instances completed."
