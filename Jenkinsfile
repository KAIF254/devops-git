pipeline {
    agent any

   stage('Test') {
    steps {
        sh 'whoami'
        sh 'ls -l /var/run/docker.sock'
        sh 'docker version'
    }

    stage('Debug') {
    steps {
        sh 'pwd'
        sh 'ls -la'
        sh 'docker ps'
        sh 'docker images'
    }
}
}
