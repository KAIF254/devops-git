pipeline {
    agent any

   stage('Test') {
    steps {
        sh 'whoami'
        sh 'ls -l /var/run/docker.sock'
        sh 'docker version'
    }
}
