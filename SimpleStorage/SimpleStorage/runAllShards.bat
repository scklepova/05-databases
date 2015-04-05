@echo off

start cmd /c "SimpleStorage -p 16000 --sp 16001,16002"
start cmd /c "SimpleStorage -p 16001 --sp 16000,16002"
start cmd /c "SimpleStorage -p 16002 --sp 16000,16001"
