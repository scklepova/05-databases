@echo off

start cmd /c "SimpleStorage -p 16000 --rp 16001,16002"
start cmd /c "SimpleStorage -p 16001 --rp 16000,16002"
start cmd /c "SimpleStorage -p 16002 --rp 16000,16001"